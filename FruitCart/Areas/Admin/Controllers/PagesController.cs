using FruitCart.Models.Data;
using FruitCart.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FruitCart.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageVM> pageList;

            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();

            }
            return View(pageList);
        }
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddPage(PageVM vmModel)
        {
            //Check modelstate
            if (!ModelState.Any())
            {
                return View(vmModel);
            }
            using (Db db = new Db())
            {
                //Declare slug
                string slug;

                //Initialize DTO
                PageDTO dto = new PageDTO();

                //DTO Title
                dto.Title = vmModel.Title;

                //Check for and set slugif need be
                if (string.IsNullOrWhiteSpace(vmModel.Slug))
                {
                    slug = vmModel.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = vmModel.Slug.Replace(" ", "-").ToLower();
                }

                //Make sure Title and Slug are Unique

                if (db.Pages.Any(x => x.Title == vmModel.Title) || (db.Pages.Any(x => x.Slug == vmModel.Slug)))
                {
                    ModelState.AddModelError("", "That Title already exist");
                }
                dto.Slug = slug;
                dto.Title = vmModel.Title;
                dto.HasSideBar = vmModel.HasSideBar;
                dto.Body = vmModel.Body;

                db.Pages.Add(dto);
                db.SaveChanges();

            }
            TempData["SM"] = "You have added a new page";
            return RedirectToAction("Addpage");


        }

        public ActionResult EditPage(int id)
        {
            PageVM model;
            using (Db db = new Db())
            {

                PageDTO dto = db.Pages.Find(id);
                if (dto == null)
                {
                    return Content("The page does not exist");
                }


                model = new PageVM(dto);

                return View(model);
            }
        }
        [HttpPost]
        public ActionResult EditPage(PageVM pageVM)
        {
            if (!ModelState.IsValid)
            {
                return View(pageVM);
            }

            using (Db db = new Db())
            {
                int id = pageVM.Id;

                string slug = "Home";

                PageDTO DTO = db.Pages.Find(id);

                DTO.Title = pageVM.Title;
                if (pageVM.Slug == "Home")
                {
                    if (string.IsNullOrWhiteSpace(pageVM.Slug))
                    {
                        slug = pageVM.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = pageVM.Slug.Replace(" ", "-").ToLower();
                    }
                }
                //Make sure Title and Slug are Unique

                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == pageVM.Title) || (db.Pages.Where(x => x.Id != id).Any(x => x.Slug == pageVM.Slug)))
                {
                    ModelState.AddModelError("", "That Title already exist");
                    return View(pageVM);
                }
                DTO.Slug = slug;

                DTO.HasSideBar = pageVM.HasSideBar;
                DTO.Body = pageVM.Body;

                db.SaveChanges();
                TempData["SM"] = "Successfully edited";
                TempData.Keep();


            }
            return RedirectToAction("EditPage");
        }

        public ActionResult PageDetails(int id)
        {
            PageVM pageVM;

            using (Db db = new Db())
            {

                PageDTO DTO = db.Pages.Find(id);

                if (DTO == null)
                {
                    return Content("The page already does not exist");
                }

                pageVM = new PageVM(DTO);

            }

            return View(pageVM);
        }

        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                PageDTO dTO = db.Pages.Find(id);

                db.Pages.Remove(dTO);
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {

                int count = 1;
                PageDTO dto;
                foreach (var pageid in id)
                {
                    dto = db.Pages.Find(pageid);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }

            }
        }
     
        public ActionResult EditSidebar()
        {
            SidebarVM model;
            using (Db db = new Db())
            {
                SidebarDTO DTO = db.Sidebars.Find(1);

                model = new SidebarVM(DTO);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                SidebarDTO dTO = db.Sidebars.Find(1);

                dTO.Body = model.Body;

                db.SaveChanges();
            }

                TempData["SM"] = "Successfully Edited";

            
            return RedirectToAction("EditSidebar"); 

        }
    }

}
