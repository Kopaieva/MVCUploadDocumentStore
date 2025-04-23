using System.Diagnostics;
using System.IO.Compression;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using MVCUploadDocumentStore.Models;

namespace MVCUploadDocumentStore.Controllers;

public class HomeController : Controller
{
   private readonly NetDbContext _context;

    public HomeController(NetDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
       var files = _context.DocStore.ToList();
        return View(files);
    }

    [HttpPost]
    public IActionResult UploadFile(IFormFile dieDatei)
    {
        if (dieDatei == null || dieDatei.Length == 0)
        {
            return Content("Keine Datei ausgewählt oder die Datei ist leer");
        }
        using (var memoryStream = new MemoryStream())
        { 
        dieDatei.CopyTo(memoryStream);
            var datei = new DocStore
            {
                DocName = dieDatei.FileName,
                DocData = memoryStream.ToArray(),
                ContentType = dieDatei.ContentType,
                ContentLength = dieDatei.Length,
                InsertionDate = DateTime.Now
            };

            _context.DocStore.Add(datei);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Download(int id)
    {
        var datei = _context.DocStore.Find(id);
        if (datei == null)
        { 
        return Content("Datei nicht gefunden");
        }

        return File(datei.DocData, datei.ContentType, datei.DocName);
    }

    public IActionResult DownloadMultiple(List<int> ids)
    {
        var files = _context.DocStore.Where(d => ids.Contains(d.DocId)).ToList();
        if (files.Count == 0)
        {
            return Content("Keine Dateien gefunden");
        }
        if (files.Count == 1)
        {
            var file = files.First();
            return File(file.DocData, file.ContentType, file.DocName);
        }

        using (var memoryStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    var zipEntry = archive.CreateEntry(file.DocName);
                    using (var zipStream = zipEntry.Open())
                    { 
                    zipStream.Write(file.DocData, 0, file.DocData.Length);
                    }
                }
            }
            return File(memoryStream.ToArray(), "application/zip", "Dateien.zip");
        }


    }
    public IActionResult DeleteMultiple(List<int> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return Content("Keine Dateien ausgewählt.");
        }

        var files = _context.DocStore.Where(d => ids.Contains(d.DocId)).ToList();
        if (files.Count == 0)
        {
            return Content("Keine Dateien gefunden.");
        }

        _context.DocStore.RemoveRange(files);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

}
