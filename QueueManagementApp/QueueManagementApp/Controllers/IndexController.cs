using QueueManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;


namespace QueueManagementApp.Controllers
{
    public class IndexController : Controller
    {
        QueueManagementEntities db = new QueueManagementEntities();

        // GET: Index
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PregledNaUslugi()
        {

            return View(db.Uslugis.ToList());
        }
        public ActionResult OdberiUsluga(int id)
        {
            string uslugaNaziv = db.Uslugis.ToList().Where(z => z.Id == id).Select(x => x.Usluga).FirstOrDefault().ToString();
            int nextRedenBroj;
            var redenBrojExsist = db.RedniBroevis.ToList().Select(x => x.Datum.ToShortDateString() == DateTime.Now.ToShortDateString()).Count();
            if (redenBrojExsist == 0)
            {
                nextRedenBroj = 1;
            }
            else
            {
                int lastRedenBroj = db.RedniBroevis.ToList().Where(x => x.Datum.ToShortDateString() == DateTime.Now.ToShortDateString()).Select(y => y.RedenBroj).Max();
                nextRedenBroj = lastRedenBroj + 1;
            }

            var redenBroj = new RedenBrojViewModel
            {
                RedenBroj = nextRedenBroj,
                Usluga = uslugaNaziv

            };
            var redniBroeviNewRecord = new RedniBroevi
            {
                UslugaId = id,
                Datum = DateTime.Now,
                RedenBroj = nextRedenBroj
            };
            db.RedniBroevis.Add(redniBroeviNewRecord);
            db.SaveChanges();

            return View(redenBroj);
        }
        public ActionResult PregledNaSalteri()
        {

            return View(db.Salteris.ToList());
        }

        public ActionResult SledenKorisnik(int id)
        {
            string salterNaziv = db.Salteris.ToList().Where(z => z.Id == id).Select(x => x.Salter).FirstOrDefault().ToString();

            var usligiSalter = db.UsligiSalteris.ToList().Where(x => x.SalterId == id).Select(y => y.UslugaId).ToList();

            var sledniKorisnici = from item in db.RedniBroevis.ToList().Where(x => x.SalterId == null).ToList()
                      where usligiSalter.Contains(item.UslugaId)
                      select item;
            if (sledniKorisnici.Count() == 0)
            {
                return View();
            }
            else
            {
                var sledenKorisnik = sledniKorisnici.Min(x => x.RedenBroj);
                var sledenKorisnikRecord = db.RedniBroevis.First(x => x.RedenBroj == sledenKorisnik);
                var redniBroeviNewRecord = new RedniBroevi
                {
                    Id = sledenKorisnikRecord.Id,
                    UslugaId = sledenKorisnikRecord.UslugaId,
                    SalterId = id,
                    Datum = sledenKorisnikRecord.Datum,
                    RedenBroj = sledenKorisnikRecord.RedenBroj
                };
                db.RedniBroevis.AddOrUpdate(redniBroeviNewRecord);
                db.SaveChanges();


                var sledenKorisnikViewModel = new SledenKorisnikViewModel
                {
                    RedenBroj = sledenKorisnik,
                    Salter = salterNaziv
                };

                return View(sledenKorisnikViewModel);

            }
         
        }
    }
}