using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Project2_NewDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project2_NewDB.Controllers
{
    public class SanPhamsController : Controller
    {
        private readonly QuanLyBanHangContext _context;

        public SanPhamsController(QuanLyBanHangContext context)
        {
            _context = context;
        }

        // GET: SanPhams
        public async Task<IActionResult> Index()
        {
            var quanLyBanHangContext = _context.SanPhams.Include(s => s.MaLoaiNavigation);
            return View(await quanLyBanHangContext.ToListAsync());
        }

        // GET: SanPhams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaLoaiNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: SanPhams/Create
        public IActionResult Create()
        {
            ViewData["MaLoai"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai");
            return View();
        }

        // POST: SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSp,TenSp,MoTa,DonGia,SoLuong,MaLoai,HinhAnhFile")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                if (sanPham.HinhAnhFile != null && sanPham.HinhAnhFile.Length > 0)
                {
                    // Tạo folder lưu ảnh: wwwroot/images/product
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/product");
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    // Tạo tên file duy nhất
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(sanPham.HinhAnhFile.FileName);
                    var filePath = Path.Combine(uploads, fileName);

                    // Lưu file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await sanPham.HinhAnhFile.CopyToAsync(fileStream);
                    }

                    // Lưu URL vào MoTa
                    sanPham.MoTa = $"/images/product/{fileName}";
                }

                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaLoai"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "MaLoai", sanPham.MaLoai);
            return View(sanPham);
        }

        // GET: SanPhams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }

            ViewData["MaLoai"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
            return View(sanPham);
        }

        // POST: SanPhams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaSp,TenSp,MoTa,DonGia,SoLuong,MaLoai,HinhAnhFile")] SanPham sanPham)
        {
            if (id != sanPham.MaSp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.SanPhams.FindAsync(id);

                    if (existingProduct == null)
                        return NotFound();

                    // Xử lý upload ảnh mới
                    if (sanPham.HinhAnhFile != null && sanPham.HinhAnhFile.Length > 0)
                    {
                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(existingProduct.MoTa))
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingProduct.MoTa.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Lưu ảnh mới
                        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/product");
                        if (!Directory.Exists(uploads))
                            Directory.CreateDirectory(uploads);

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(sanPham.HinhAnhFile.FileName);
                        var filePath = Path.Combine(uploads, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await sanPham.HinhAnhFile.CopyToAsync(fileStream);
                        }

                        existingProduct.MoTa = $"/images/product/{fileName}";
                    }

                    // Cập nhật các thông tin còn lại
                    existingProduct.TenSp = sanPham.TenSp;
                    existingProduct.DonGia = sanPham.DonGia;
                    existingProduct.SoLuong = sanPham.SoLuong;
                    existingProduct.MaLoai = sanPham.MaLoai;

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSp))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaLoai"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
            return View(sanPham);
        }

        // Private helper method
        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.MaSp == id);
        }
        // GET: SanPhams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var sanPham = await _context.SanPhams
                .Include(s => s.MaLoaiNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);

            if (sanPham == null)
                return NotFound();

            return View(sanPham);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
                return NotFound();

            // Lưu đường dẫn file ảnh để xóa sau nếu database xóa thành công
            string filePath = null;
            if (!string.IsNullOrEmpty(sanPham.MoTa))
            {
                var relativePath = sanPham.MoTa.TrimStart('/');
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.Replace('/', Path.DirectorySeparatorChar));
            }

            try
            {
                // Thử xóa sản phẩm khỏi database
                _context.SanPhams.Remove(sanPham);
                await _context.SaveChangesAsync();

                // Nếu database xóa thành công, xóa file ảnh
                if (filePath != null && System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch
                    {
                        // Bỏ qua lỗi xóa file ảnh
                    }
                }
            }
            catch (DbUpdateException)
            {
                // Lỗi khóa ngoại
                TempData["Error"] = "Cannot delete product because it is referenced by other records. Please remove related data first.";
                return RedirectToAction(nameof(Delete), new { id });
            }
            catch (Exception ex)
            {
                // Các lỗi khác
                TempData["Error"] = "Cannot delete product: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id });
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
