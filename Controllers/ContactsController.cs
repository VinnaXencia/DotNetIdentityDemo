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

        public ContactsController(ILogger<ContactsController> logger, DotNetBrushUpDbContext dbContext, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, IConfiguration configuration)
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
            //Removing the prefix upto where the Underscore is present
            if (contactDataModel.ContactProofFileName != null)
            {
                int underscoreIndex = contactDataModel.ContactProofFileName.IndexOf('_');
                contactDataModel.ContactProofFileName = contactDataModel.ContactProofFileName.Substring(underscoreIndex + 1);
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

        //Add New Contact
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
            //return RedirectToAction("Create", formData);
            return RedirectToAction("Index");
        }

        //Update Existing Contact
        
        public async Task<IActionResult> UpdateContact(ContactDataModel formData)
        {
            //var contactToUpdate = await _dbContext.ContactsDataModel.FindAsync(formData.ContactId);
            var proofFile = formData.ContactProofFile;

            var contactToUpdate = _dbContext.ContactsDataModel.SingleOrDefault(c => c.ContactId == formData.ContactId);

            if (formData.ContactProofFile != null)
            {
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
                contactToUpdate.ContactProofFilePath = formData.ContactProofFilePath;
                contactToUpdate.ContactProofFileName = formData.ContactProofFileName;
            }                          

            contactToUpdate.ContactName = formData.ContactName;
            contactToUpdate.ContactEmail = formData.ContactEmail;
            contactToUpdate.ContactAddress = formData.ContactAddress;
            contactToUpdate.ContactPhoneNo = formData.ContactPhoneNo;            
            _dbContext.Entry(contactToUpdate).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = formData.ContactId });
            //return RedirectToAction("Index");
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

            //Removing the prefix upto where the Underscore is present
            if (contact.ContactProofFileName != null)
            {
                int underscoreIndex = contact.ContactProofFileName.IndexOf('_');
                contact.ContactProofFileName = contact.ContactProofFileName.Substring(underscoreIndex + 1);
            }

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
            //Removing the prefix upto where the Underscore is present
            if (contactDataModel.ContactProofFileName != null)
            {
                int underscoreIndex = contactDataModel.ContactProofFileName.IndexOf('_');
                contactDataModel.ContactProofFileName = contactDataModel.ContactProofFileName.Substring(underscoreIndex + 1);
            }

            return View(contactDataModel);
        }

        

        //Live Search Functionality
        [HttpGet]
        public async Task<IActionResult> SearchContacts(string searchText)
        {
            var userId = _userManager.GetUserId(this.User);

            if (string.IsNullOrEmpty(searchText))
            {
                var contacts = await _dbContext.ContactsDataModel.ToListAsync();
                return PartialView("_ContactListPartial", contacts);
            }

            var filteredContacts = await _dbContext.ContactsDataModel
                .Where(contact =>
                    contact.ContactName.Contains(searchText) || // Search by Name
                    contact.ContactPhoneNo.Contains(searchText)) // Search by Phone No
                .ToListAsync();

            return PartialView("_ContactListPartial", filteredContacts); // Create a partial view to render the contacts list
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
