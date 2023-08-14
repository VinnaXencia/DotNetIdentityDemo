using Azure.Storage.Blobs;
using DotNetBrushUp.Areas.Identity.Data;
using DotNetBrushUp.Data;
using DotNetBrushUp.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DotNetBrushUp.Controllers
{
    //[ApiController]
    //[Route("api/[controller]")]
    [Authorize]
    public class ContactsController : Controller
    {
        private readonly ILogger<ContactsController> _logger;
        private readonly DotNetBrushUpDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public ContactsController(ILogger<ContactsController> logger,DotNetBrushUpDbContext dbContext, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _logger = logger;
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
            this._userManager = userManager;
            _configuration = configuration;
        }

        // GET: Contacts
        //[Route("Index")]
        public async Task<IActionResult> Index()
        {
            ViewData["UserID"] = _userManager.GetUserId(this.User);
            var userId = _userManager.GetUserId(this.User);
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

        //[HttpPost]
        ////[Route("Contacts/AddContact")]
        //public async Task<IActionResult> AddContact(ContactDataModel formData)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ProofFiles");
        //        string uniqueFileName = Guid.NewGuid().ToString() + "_" + formData.ContactProofFile.FileName;
        //        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await formData.ContactProofFile.CopyToAsync(fileStream);
        //        }

        //        formData.ContactProofFilePath = filePath;
        //        formData.ContactProofFileName = uniqueFileName;

        //    }
        //    _dbContext.ContactsDataModel.Add(formData);
        //    await _dbContext.SaveChangesAsync();
        //    return RedirectToAction("Create", formData);
        //}

        [HttpPost]
        public async Task<IActionResult> AddContact(ContactDataModel formData)
        {
            if (ModelState.IsValid)
            {
                //string uniqueFileName = Guid.NewGuid().ToString() + "_" + formData.ContactProofFile.FileName;
                string uniqueFileName = DateTime.Now.ToString("ddMMyyHHmmss") + "_" + formData.ContactProofFile.FileName;

                BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("AzureBlobStorageConnection"));
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("contactsproof");

                BlobClient blobClient = containerClient.GetBlobClient(uniqueFileName);

                using (var stream = formData.ContactProofFile.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

                formData.ContactProofFilePath = blobClient.Uri.ToString();
                formData.ContactProofFileName = uniqueFileName;
            }

            _dbContext.ContactsDataModel.Add(formData);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Create", formData);
        }

        public async Task<IActionResult> DownloadProofFile(int contactId)
        {
            var contact = await _dbContext.ContactsDataModel.FindAsync(contactId);
            if (contact == null || string.IsNullOrEmpty(contact.ContactProofFileName))
            {
                return NotFound();
            }

            // Create a BlobServiceClient using your connection string
            BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("AzureBlobStorageConnection"));

            // Get a reference to the blob container
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("contactsproof");

            // Get a reference to the blob
            BlobClient blobClient = containerClient.GetBlobClient(contact.ContactProofFileName);

            // Get the blob's content as a stream
            var blobStream = await blobClient.OpenReadAsync();

            // Return the blob's content as a FileResult
            return File(blobStream, "application/octet-stream", contact.ContactProofFileName);
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
