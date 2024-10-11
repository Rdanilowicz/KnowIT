﻿using KnowIT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KnowIT.Controllers
{
	public class CategoryController : Controller
	{
		private readonly KnowledgeDbContext _context;
		private readonly ILogger<CategoryController> _logger;

		public CategoryController(ILogger<CategoryController> logger, KnowledgeDbContext context)
		{
			_context = context;
			_logger = logger;
		}

		// GET: Categories
		public async Task<IActionResult> Index()
		{
			var categories = await _context.Categories.ToListAsync();
			return View(categories);
		}

		// GET: Category/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Category/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Name")] Category category)
		{
			if (ModelState.IsValid)
			{
				_context.Add(category);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(category);
		}

		// GET: Category/Edit
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var category = await _context.Categories.FindAsync(id);
			if (category == null)
			{
				return NotFound();
			}
			return View(category);
		}

		// POST: Category/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
		{
			if (id != category.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(category);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!CategoryExists(category.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction("Index");
			}
			return View(category);
		}

		// GET: Category/Delete
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var category = await _context.Categories
				.FirstOrDefaultAsync(m => m.Id == id);
			if (category == null)
			{
				return NotFound();
			}

			return View(category);
		}

		// POST: Category/Delete
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		private bool CategoryExists(int id)
		{
			return _context.Categories.Any(e => e.Id == id);
		}
	}
}