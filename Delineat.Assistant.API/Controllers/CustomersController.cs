using Delineat.Assistant.API.Models;
using Delineat.Assistant.Core.Data;
using Delineat.Assistant.Core.Data.Models;
using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.ObjectFactories;
using Delineat.Assistant.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Delineat.Assistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : BaseController
    {
        private readonly DADWObjectFactory dwObjectFactory;

        public DAAssistantDBContext assistantDBContext { get; }

        public CustomersController(DAAssistantDBContext assistantDBContext, ILogger<DayWorkTypesController> logger) : base(logger)
        {
            this.dwObjectFactory = new DADWObjectFactory(assistantDBContext);
            this.assistantDBContext = assistantDBContext;
        }

        [HttpGet()]
        public ActionResult<DWCustomer[]> GetCustomers()
        {
            try
            {
                return this.assistantDBContext.Customers.Where(c=> c.DeleteDate == null).Select(c => dwObjectFactory.GetDWCustomer(c)).ToArray();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<DWCustomer> GetCustomer(int id)
        {
            try
            {
                var customer = assistantDBContext.Customers
                    .Where(c => c.CustomerId == id)
                    .Select(c => dwObjectFactory.GetDWCustomer(c)).FirstOrDefault();
                if(customer != null)
                {
                    return customer;
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }

        private bool Validate(Customer customer)
        {
            if (customer == null) ModelState.AddModelError(nameof(customer), "Cliente non valorizzato");
           
            return ModelState.IsValid;
        }

        private bool Validate(CustomerRequest data, int? id = null)
        {
            if (data == null) ModelState.AddModelError(nameof(data), "Cliente non valorizzato");
            if (string.IsNullOrWhiteSpace(data.Code))
            {
                ModelState.AddModelError(nameof(data.Code), "Codice cliente non valorizzato");
            }
            else
            {
                if (data.Code.Trim().Length > 2) ModelState.AddModelError(nameof(data.Code), "Il codice cliente deve essere lungo 2 caratteri");
                var customer = assistantDBContext.Customers.FirstOrDefault(c => c.Code == data.Code && c.CustomerId != (id ?? -1));
                if(customer != null)
                {
                    ModelState.AddModelError(nameof(data.Code), $"Codice cliente già utilizzato per cliente '{customer.Description}'");
                }
            }
            if (string.IsNullOrWhiteSpace(data.Description))
            {
                ModelState.AddModelError(nameof(data.Description), "Descrizione cliente non valorizzata");
            }
           


            return ModelState.IsValid;
        }

        private Customer FillFromRequest(Customer customer, CustomerRequest data)
        {

            customer.Code = data.Code;
            customer.Domain = data.Domain;
            customer.Description = data.Description;
           
            return customer;
        }


        [HttpPut("{id}")]
        public ActionResult<DWCustomer> UpdateCustomer(int id, CustomerRequest data)
        {
            try
            {
                if (Validate(data,id))
                {

                    var customer = assistantDBContext.Customers.FirstOrDefault(c => c.CustomerId == id);

                    if (customer != null)
                    {
                        if (Validate(FillFromRequest(customer, data)))
                        {

                            assistantDBContext.Customers.Update(customer);
                            assistantDBContext.SaveChanges();

                            return dwObjectFactory.GetDWCustomer(customer);
                        }
                        else
                        {
                            return BadRequest(ModelState);
                        }
                    }
                    else
                    {
                        return NotFound();
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

        [HttpPost()]
        public ActionResult<DWCustomer> InsertCustomer(CustomerRequest data)
        {
            try
            {
                if (Validate(data))
                {                                      

                    var customer = new Customer();
                    if (Validate(FillFromRequest(customer, data)))
                    {

                        assistantDBContext.Customers.Add(customer);
                        assistantDBContext.SaveChanges();

                        return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, dwObjectFactory.GetDWCustomer(customer));
                    }
                    else
                    {
                        return BadRequest(ModelState);
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


        [HttpDelete("{id}")]
        public ActionResult DeleteCustomer(int id)
        {
            try
            {
                var customer = assistantDBContext.Customers.Include(c=> c.Jobs).Include(c=>c.Notes).FirstOrDefault(c => c.CustomerId == id);
                if (customer != null)
                {
                    // Verifico se il cliente ha delle commesse collegate
                    // Se si lo annullo logicamente altrimenti lo rimuovo
                    if(customer.Jobs != null && customer.Jobs.Count() > 0) {
                        customer.DeleteDate = DateTime.Now;
                        assistantDBContext.Customers.Update(customer);
                    }
                    else
                    {
                        foreach(var note in customer.Notes)
                        {
                            assistantDBContext.CustomersNotes.Remove(note);
                        }
                        assistantDBContext.Customers.Remove(customer);
                    }
                    
                    assistantDBContext.SaveChanges();
                    return Ok();
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }


    }
}
