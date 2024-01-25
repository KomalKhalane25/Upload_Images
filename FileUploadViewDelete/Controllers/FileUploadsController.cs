using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FileUploadViewDelete.Models;
using Rotativa;

namespace FileUploadViewDelete.Controllers
{
    public class FileUploadsController : Controller
    {
        private UploadFileEntities db = new UploadFileEntities();

        // GET: FileUploads
        public ActionResult Index()
        {
            var UploadFileEntities = db.FileUploads.ToList();
            return View(UploadFileEntities);
            
        }
        public ActionResult PrintViewToPdf()
        {
            var report = new ActionAsPdf("Index");
            return report;
        }
        public ActionResult PrintPartialViewToPdf(int id)
        {
            using (UploadFileEntities db = new UploadFileEntities())
            {
                FileUpload customer = db.FileUploads.FirstOrDefault(c => c.Id == id);

                var report = new PartialViewAsPdf("~/Views/DailyCalling/Details.cshtml", customer);
                return report;
            }

        }
        // GET: FileUploads/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileUpload fileUpload = db.FileUploads.Find(id);
            if (fileUpload == null)
            {
                return HttpNotFound();
            }
            return View(fileUpload);
        }

        // GET: FileUploads/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FileUploads/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( FileUpload s)
        {
            if (s.Image != null)
            {
                string extension = Path.GetExtension(s.Image.FileName).ToLower();

                if (IsImageExtensionValid(extension))
                {
                    if (s.Image.ContentLength <= 1000000)
                    {
                        string fileName = Guid.NewGuid().ToString() + extension;
                        string filePath = Path.Combine(Server.MapPath("~/Image/"), fileName);

                        s.Image.SaveAs(filePath);
                        s.FilePath = "~/Image/" + fileName;

                        db.FileUploads.Add(s);
                        int result = db.SaveChanges();

                        if (result > 0)
                        {
                            TempData["InsertMessage"] = "<script> alert('Record Inserted Successfully !!') </script>";
                            ModelState.Clear();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ViewBag.Message = "Record Not Inserted";
                        }
                    }
                    else
                    {
                        ViewBag.SizeMessage = "File size should be 1 MB or less.";
                    }
                }
                else
                {
                    ViewBag.ExtensionMessage = "Image file format not supported. Only JPG, JPEG, and PNG are allowed.";
                }
            }
            else
            {
                ViewBag.ExtensionMessage = "Please upload an ImageFile";
            }

            //  If model state is not valid, return the view with the model
            return View(s);
        }

        private bool IsImageExtensionValid(string extension)
        {
            // You can extend this list as needed
            string[] validExtensions = { ".jpg", ".jpeg", ".png" };
            return validExtensions.Contains(extension);
        }

    

    // GET: FileUploads/Edit/5
    public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileUpload fileUpload = db.FileUploads.Find(id);
            if (fileUpload == null)
            {
                return HttpNotFound();
            }
            return View(fileUpload);
        }

        // POST: FileUploads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Number,DateTime,FilePath,FileName")] FileUpload fileUpload)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fileUpload).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fileUpload);
        }

        // GET: FileUploads/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileUpload fileUpload = db.FileUploads.Find(id);
            if (fileUpload == null)
            {
                return HttpNotFound();
            }
            return View(fileUpload);
        }

        // POST: FileUploads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FileUpload fileUpload = db.FileUploads.Find(id);
            db.FileUploads.Remove(fileUpload);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
