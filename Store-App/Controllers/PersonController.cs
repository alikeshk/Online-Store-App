﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_App.Controllers.Interfaces;
using Store_App.Helpers;
using Store_App.Models.Authentication;
using Store_App.Models.DBClasses;
using System.Data;

namespace Store_App.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class PersonController : ControllerBase, IPersonController
    {
        private readonly StoreAppDbContext _personContext;

        public PersonController(StoreAppDbContext personContext)
        {
            _personContext = personContext;
        }

        [HttpPost]
        public bool Login(LoginRequest request)
        {
            Person? user = (from p in _personContext.People
                            where p.Email == request.Email && p.Password == request.Password
                            select p).FirstOrDefault();
            if (user != null)
            {
                UserHelper.SetCurrentUser(user);
                return true;
            }
            return false;
        }

        [HttpGet]
        public bool Logout()
        {
            UserHelper.SetCurrentUser(null);
            return true;
        }

        [HttpGet("{personId}")]
        public async Task<ActionResult<Person>> GetPerson(int personId)
        {
            var person = await _personContext.People.FindAsync(personId);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        [HttpPost]
        public async Task<ActionResult<Person>> CreatePerson(Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _personContext.People.Add(person);
            await _personContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerson), new { personId = person.PersonId }, person);
        }

        [HttpPut("{personId}")]
        public async Task<ActionResult> UpdatePerson(int personId, Person person)
        {
            if (personId != person.PersonId)
            {
                return BadRequest();
            }

            _personContext.Entry(person).State = EntityState.Modified;

            try
            {
                await _personContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok();
        }

        [HttpDelete("{personId}")]
        public async Task<ActionResult> DeletePerson(int personId)
        {
            var person = await _personContext.People.FindAsync(personId);

            if (person == null)
            {
                return NotFound();
            }

            _personContext.People.Remove(person);
            await _personContext.SaveChangesAsync();

            return Ok();
        }
    }
}
