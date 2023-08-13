﻿using DotNetBrushUp.Areas.Identity.Data;
using DotNetBrushUp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DotNetBrushUp.DataModels;
using DotNetBrushUp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;

namespace DotNetBrushUp.Controllers
{
    //[ApiController]
    //[Route("api/[controller]")]
    //[Route("Contacts")]
    public class ContactsController : Controller
    {
        private readonly ILogger<ContactsController> _logger;
        private readonly DotNetBrushUpDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContactsController(ILogger<ContactsController> logger,DotNetBrushUpDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Contacts
        //[Route("Index")]
        public async Task<IActionResult> Index()
        {
              return _dbContext.ContactsDataModel != null ? 
                          View(await _dbContext.ContactsDataModel.ToListAsync()) :
                          Problem("Entity set 'DotNetBrushUpDbContext.ContactsDataModel'  is null.");
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _dbContext.ContactsDataModel == null)
            {
                return NotFound();
            }

            var contactDataModel = await _dbContext.ContactsDataModel
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (contactDataModel == null)
            {
                return NotFound();
            }

            return View(contactDataModel);
        }

        // GET: Contacts/Create
        //[HttpGet]
        //[Route("Contacts/Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        //[Route("Contacts/AddContact")]
        public async Task<IActionResult> AddContact(ContactDataModel formData)
        {
            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ProofFiles");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + formData.ContactProofFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await formData.ContactProofFile.CopyToAsync(fileStream);
                }

                formData.ContactProofFilePath = filePath;
                formData.ContactProofFileName = uniqueFileName;

            }
            _dbContext.ContactsDataModel.Add(formData);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Create", formData);
        }


        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContactId,ContactName,ContactEmail,ContactAddress,ContactPhoneNo,ContactProofFilePath,ContactProofFileName")] ContactDataModel contactDataModel)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Add(contactDataModel);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contactDataModel);
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _dbContext.ContactsDataModel == null)
            {
                return NotFound();
            }

            var contactDataModel = await _dbContext.ContactsDataModel.FindAsync(id);
            if (contactDataModel == null)
            {
                return NotFound();
            }
            return View(contactDataModel);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ContactId,ContactName,ContactEmail,ContactAddress,ContactPhoneNo,ContactProofFilePath,ContactProofFileName")] ContactDataModel contactDataModel)
        public async Task<IActionResult> UpdateContact(ContactDataModel formData)
        {
            if (ModelState.IsValid)
            {
                //string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ProofFiles");
                //string uniqueFileName = Guid.NewGuid().ToString() + "_" + formData.ContactProofFile.FileName;
                //string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                //using (var fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    await formData.ContactProofFile.CopyToAsync(fileStream);
                //}

                //formData.ContactProofFilePath = filePath;
               // formData.ContactProofFileName = uniqueFileName;

            }
            var contactToUpdate = await _dbContext.ContactsDataModel.FindAsync(formData.ContactId);

            if (contactToUpdate == null)
            {
                return NotFound(); // Handle the case when the contact is not found
            }

            contactToUpdate.ContactName = formData.ContactName;
            contactToUpdate.ContactEmail = formData.ContactEmail;
            contactToUpdate.ContactAddress = formData.ContactAddress;
            contactToUpdate.ContactPhoneNo = formData.ContactPhoneNo;
            _dbContext.Entry(contactToUpdate).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Edit", new { id = formData.ContactId });
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _dbContext.ContactsDataModel == null)
            {
                return NotFound();
            }

            var contactDataModel = await _dbContext.ContactsDataModel
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (contactDataModel == null)
            {
                return NotFound();
            }

            return View(contactDataModel);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_dbContext.ContactsDataModel == null)
            {
                return Problem("Entity set 'DotNetBrushUpDbContext.ContactsDataModel'  is null.");
            }
            var contactDataModel = await _dbContext.ContactsDataModel.FindAsync(id);
            if (contactDataModel != null)
            {
                _dbContext.ContactsDataModel.Remove(contactDataModel);
            }
            
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactDataModelExists(int id)
        {
          return (_dbContext.ContactsDataModel?.Any(e => e.ContactId == id)).GetValueOrDefault();
        }
    }
}
