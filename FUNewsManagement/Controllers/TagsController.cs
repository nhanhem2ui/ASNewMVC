using BussinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace FUNewsManagement.Controllers
{

    [Authorize(Roles = "Admin,Staff,Lecturer")]
    public class TagsController : Controller
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        // GET: Tags
        public IActionResult Index(string? filter)
        {
            var tags = _tagService.GetTags();

            if (!string.IsNullOrEmpty(filter))
            {
                tags = tags.Where(t => t.TagName.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(tags);
        }

        // GET: Tags/Details/5
        public IActionResult Details(int id)
        {
            var tag = _tagService.GetTagById(id);
            if (tag == null)
                return NotFound();

            return View(tag);
        }

        // GET: Tags/Create
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Create() => View(new Tag());

        // POST: Tags/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Create(Tag tag)
        {
            if (!ModelState.IsValid)
                return View(tag);

            try
            {
                _tagService.SaveTag(tag);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to create tag: {ex.Message}");
                return View(tag);
            }
        }

        // GET: Tags/Edit/5
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Edit(int id)
        {
            var tag = _tagService.GetTagById(id);
            if (tag == null)
                return NotFound();

            return View(tag);
        }

        // POST: Tags/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Edit(int id, Tag tag)
        {
            if (id != tag.TagId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(tag);

            try
            {
                _tagService.UpdateTag(tag);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to update tag: {ex.Message}");
                return View(tag);
            }
        }

        // GET: Tags/Delete/5
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Delete(int id)
        {
            var tag = _tagService.GetTagById(id);
            if (tag == null)
                return NotFound();

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _tagService.DeleteTag(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to delete tag: {ex.Message}");
                var tag = _tagService.GetTagById(id);
                return View("Delete", tag);
            }
        }
    }
}

