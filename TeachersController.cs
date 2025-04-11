using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using OlympWeb.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using static System.Net.Mime.MediaTypeNames;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using DocumentFormat.OpenXml;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;


namespace OlympWeb.Controllers
{
    public class TeachersController : Controller
    {
        private OlympEntities1 db = new OlympEntities1();

        // GET: Teachers
        public ActionResult Index()
        {
            return View(db.Teachers.ToList());
        }

        // GET: Teachers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teachers teachers = db.Teachers.Find(id);
            if (teachers == null)
            {
                return HttpNotFound();
            }
            return View(teachers);
        }

        // GET: Teachers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_teacher,user_id,first_name,last_name,middle_name,date_of_birth,department,disciplines")] Teachers teachers)
        {
            if (ModelState.IsValid)
            {
                db.Teachers.Add(teachers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(teachers);
        }

        // GET: Teachers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teachers teachers = db.Teachers.Find(id);
            if (teachers == null)
            {
                return HttpNotFound();
            }
            return View(teachers);
        }

        // POST: Teachers/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_teacher,user_id,first_name,last_name,middle_name,date_of_birth,department,disciplines")] Teachers teachers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teachers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teachers);
        }

        // GET: Teachers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teachers teachers = db.Teachers.Find(id);
            if (teachers == null)
            {
                return HttpNotFound();
            }
            return View(teachers);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Teachers teachers = db.Teachers.Find(id);
            db.Teachers.Remove(teachers);
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

        // Метод для отображения результатов студентов
        public ActionResult ViewResults()
        {
            if (Session["UserID"] == null || Convert.ToInt32(Session["Role"]) != 1)
            {
                return RedirectToAction("Login", "Account"); // Перенаправление, если не авторизован или не преподаватель
            }

            var results = db.Test_result
             .Include(t => t.Students)
             .Include(t => t.Students.Users) // <-- ВАЖНО!
             .Include(t => t.Tests) // если выводишь название теста
             .ToList();
            // Подгружаем данные о студенте
            return View(results);
        }


        [HttpPost]
        public ActionResult ExportCsv(string exportData)
        {
            if (string.IsNullOrEmpty(exportData))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Нет данных для экспорта");
            }

            var rows = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(exportData);
            StringBuilder sb = new StringBuilder();

            foreach (var row in rows)
            {
                sb.AppendLine(string.Join(" | ", row));
            }

            // Добавляем BOM перед UTF-8
            var preamble = Encoding.UTF8.GetPreamble(); // это и есть BOM: EF BB BF
            var contentBytes = Encoding.UTF8.GetBytes(sb.ToString());

            byte[] finalBytes = new byte[preamble.Length + contentBytes.Length];
            Buffer.BlockCopy(preamble, 0, finalBytes, 0, preamble.Length);
            Buffer.BlockCopy(contentBytes, 0, finalBytes, preamble.Length, contentBytes.Length);

            return File(finalBytes, "text/csv", "результаты.csv");
        }




        [HttpPost]
        public ActionResult ExportWord(string exportData)
        {
            if (string.IsNullOrEmpty(exportData))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Нет данных для экспорта");
            }

            var rows = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(exportData);
            if (rows.Count == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Нет строк данных");
            }

            var headers = rows[0]; // ✅ Первая строка — это заголовки
            var dataRows = rows.Skip(1).ToList(); // ✅ Остальные строки — данные

            using (var mem = new MemoryStream())
            {
                using (var wordDoc = WordprocessingDocument.Create(mem, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = new Body();

                    Table table = new Table();

                    TableProperties tblProperties = new TableProperties(
                        new TableBorders(
                            new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                            new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                            new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                            new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                            new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                            new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
                        )
                    );
                    table.AppendChild(tblProperties);

                    // ✅ Добавляем строку заголовков
                    TableRow headerRow = new TableRow();
                    foreach (var header in headers)
                    {
                        TableCell tc = new TableCell(new Paragraph(new Run(new RunProperties(new Bold()), new Text(header))));
                        headerRow.Append(tc);
                    }
                    table.Append(headerRow);

                    // ✅ Добавляем данные
                    foreach (var row in dataRows)
                    {
                        TableRow tr = new TableRow();
                        foreach (var cellText in row)
                        {
                            TableCell tc = new TableCell(new Paragraph(new Run(new Text(cellText ?? string.Empty))));
                            tr.Append(tc);
                        }
                        table.Append(tr);
                    }

                    body.Append(table);
                    mainPart.Document.Append(body);
                    mainPart.Document.Save();
                }

                mem.Seek(0, SeekOrigin.Begin);
                return File(mem.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "результаты.docx");
            }
        }







    }
}
