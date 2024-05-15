using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Pronia.DataAccesLayer;
using Pronia.Extensions;
using Pronia.Models;
using Pronia.ViewModels.Products;
using System.Text;

namespace Pronia.Areas.Admin.Controllers;

[Area("Admin")]
    public class ProductController(ProniaContext _context,IWebHostEnvironment _env) : Controller
    {
        public async Task<IActionResult> Index()
        {
        
            return View(await _context.Products.Select(p=>new GetProdactAdminVM
            {
                CostPrice = p.CostPrice,
                Discount = p.Discount,
                Id = p.Id,
                Name = p.Name,
                Raiting = p.Raiting,
                SellPrice = p.SellPrice,
                StockCount = p.StockCount,
                ImageUrl=p.IamgeUrl

            }).ToListAsync());
        }

           public async Task<IActionResult> Create()
           {
                ViewBag.Categories = await _context.Categories
                .Where(s => !s.IsDeleted)
                .ToListAsync();
        return View();
           }
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductVM data)
    {
        if (data.ImageFile!=null)
        {

        
            if (!data.ImageFile.IsValidType("image"))
            { 
            ModelState.AddModelError("ImageFile", "seki sehvdir");
            }
            if(data.ImageFile.IsValidLength(20))
            {
            ModelState.AddModelError("ImageFile", "sekil boyukdur");
            }
        }
        byte count = 0;
        bool isImageValid = true;
        StringBuilder sb=new StringBuilder();
        foreach (var img in data.ImageFiles  ?? new List<IFormFile>())
        {
            if (!img.IsValidType("Image"))
            {
                sb.Append((count++) + img.FileName + "file sehvdir");
                //ModelState.AddModelError("ImageFiles",img.FileName+"faylin formati duzgun deyil");
                isImageValid = false;
            }
            if (img.IsValidLength(200)) 
            {
                sb.Append((count++) + img.FileName + "size coxdur");
                //ModelState.AddModelError("ImageFiles",img.FileName+"faylin olcusu 20kb-dan coxdur")
                isImageValid = false;
            }

        }
        if (!isImageValid)
        {
           
            ModelState.AddModelError("ImageFiles", sb.ToString());
        }

        if (await _context.Categories.CountAsync(c=>data.CategoryIds.Contains(c.Id))!=data.CategoryIds.Length)
        {
            ModelState.AddModelError("CategoryIds", "Kateqoriya tapilmadi");

        }



        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories
                .Where(s => !s.IsDeleted)
                .ToListAsync();
            return View();
        }
       
        string fileName = await data.ImageFile.SaveFileAsync(Path.Combine(_env.WebRootPath, "imgs", "products"));
        Product product = new Product
        {
            CostPrice = data.CostPrice,
            CreatedTime = DateTime.Now,
            Discount = data.Discount,
            IamgeUrl = Path.Combine("imgs", "products", fileName),
            IsDeleted = false,
            Name = data.Name,
            Raiting = data.Raiting,
            SellPrice = data.SellPrice,
            StockCount = data.StockCount,
            Images=new List<ProductImage>()
        };
        
        foreach (var img in data.ImageFiles)
        {
            string imgName = await img.SaveFileAsync(Path.Combine(_env.WebRootPath, "imgs", "products"));
            product.Images.Add(new ProductImage
            {
                ImageUrl=Path.Combine("imgs","products",imgName),
                CreatedTime=DateTime.Now,
                IsDeleted = false,
            });
        }

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
 
     }

