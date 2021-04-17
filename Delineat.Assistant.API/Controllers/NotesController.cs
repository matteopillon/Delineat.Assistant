using Delineat.Assistant.API.Models;
using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.Tips.Extensions;
using Delineat.Assistant.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Delineat.Assistant.Core.Data;
using Delineat.Assistant.Core.ObjectFactories;
using Delineat.Assistant.API.Helpers;

namespace Delineat.Assistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : StoreBaseController
    {
        private readonly DADWObjectFactory dwObjectFactory;

        public readonly DAAssistantDBContext assistantDBContext;
        public NotesController(DAAssistantDBContext assistantDBContext, IDAStore store, ILogger<WorkLogsController> logger) : base(store, logger)
        {
            this.dwObjectFactory = new DADWObjectFactory(assistantDBContext);
            this.assistantDBContext = assistantDBContext;
        }

        private bool Validate(DWNote note)
        {
            if (note == null) ModelState.AddModelError(nameof(note), "Nota non valorizzata");
            if (string.IsNullOrWhiteSpace(note.Note))
            {
                ModelState.AddModelError(nameof(DWNote.Note), "Testo nota non valorizzato");
            }

            if (note.IsRemainder)
            {
                if(note.RemainderDate == null)
                {
                    ModelState.AddModelError(nameof(DWNote.RemainderDate), "La data e ora del promemoria non è valorizzata");
                }
                if (note.RemainderDate < DateTime.Now)
                {
                    ModelState.AddModelError(nameof(DWNote.RemainderDate), "La data e ora del promemoria non può essere passata");
                }
                if (note.Emails.Length == 0)
                {
                    ModelState.AddModelError(nameof(DWNote.Emails), "Per il promemoria deve essere specificata almeno una email");
                }
                else
                {
                    foreach (var email in note.Emails)
                    {
                        if (!email.IsEmail())
                        {
                            ModelState.AddModelError(nameof(DWNote.Emails), $"L'indirizzo email '{email}' non è valido");
                        }
                    }
                }
            }

            return ModelState.IsValid;
        }




        private DWNote FillFromRequest(DWNote note, DANoteApiRequest data, int? referenceId = null)
        {
            if (data.remainderDateTime.Kind == DateTimeKind.Utc)
            {
                note.RemainderDate = data.remainderDateTime.ToLocalTime();
            }
            else
            {
                note.RemainderDate = data.remainderDateTime;
            }

            note.Note = data.Note;
            note.Emails = data.Emails ?? new string[0];
            note.IsRemainder = data.IsRemainder;
            
            return note;
        }


        [HttpPost("items/{itemId}")]
        public ActionResult<DWNote> AddItemNote(int itemId, DANoteApiRequest data)
        {
            try
            {
                //Verifico l'esistenza dell'item
                var item = assistantDBContext.GetItem(itemId);
                if (item == null) return NotFound();

                var note = FillFromRequest(new DWNote() { NoteType = NoteType.Item }, data);

                if (Validate(note))
                {
                    var result = Store.AddNoteToItem(itemId, note);
                    if (result.Stored)
                    {
                        return note;
                    }
                    else
                    {
                        return BadRequest(result.ErrorMessages);
                    }

                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }

        }
    }
}
