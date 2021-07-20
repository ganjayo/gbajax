using gbajax.Models;
using gbajax.Service;
using gbajax.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace gbajax.Controllers
{
    public class GuestbooksController : Controller

    {

        private readonly GuestbooksDBService GuestbooksService = new GuestbooksDBService();
        // GET: Guestbooks
        
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult GetDataList(string Search, int Page =1)
        {
            GuestbooksViewModel Data = new GuestbooksViewModel();
            Data.Search = Search;
            Data.Paging = new ForPaging(Page);
            Data.DataList = GuestbooksService.GetDataList(Data.Paging, Data.Search);
            return PartialView(Data);
        }
        [HttpPost]

        public ActionResult GetDataList([Bind(Include ="SEARCH")] GuestbooksViewModel Data)
        {
            return RedirectToAction("GetDataList", new { Search = Data.Search });
        }
        public ActionResult Create()
        {
            return PartialView();
        }
        [Authorize]
        [HttpPost]
        public ActionResult Create([Bind(Include ="CONTENT")] Guestbooks Data )
        {
            Data.ACCOUNT = User.Identity.Name;
            GuestbooksService.InsertGuestbooks(Data);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int ID)
        {
            Guestbooks Data = GuestbooksService.GetDataByID(ID);
            return View(Data);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int ID, [Bind(Include = "CONTENT")] Guestbooks UpdateData)
        {
            if (GuestbooksService.CheckUpdate(ID))
            {
                UpdateData.ID = ID;
                UpdateData.ACCOUNT = User.Identity.Name;
                GuestbooksService.UpdateGuestbooks(UpdateData);
                return RedirectToAction("Index");
            }

            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Reply(int ID)
        {
            Guestbooks Data = GuestbooksService.GetDataByID(ID);
            return View(Data);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Reply (int ID, [Bind(Include = "REPLY,REPLYTIME")] Guestbooks ReplyData)
        {
            if (GuestbooksService.CheckUpdate(ID))
            {
                ReplyData.ID = ID;
                GuestbooksService.ReplyGuestbooks(ReplyData);
                return RedirectToAction("Index");
            }

            else
            {
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Delete(int ID)
        {

            GuestbooksService.DeletGuestbooks(ID);
            return RedirectToAction("Index");
        }


    }
}