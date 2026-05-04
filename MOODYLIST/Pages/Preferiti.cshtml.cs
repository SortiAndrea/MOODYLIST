using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MOODYLIST.Data;
using MOODYLIST.Model;
using System.Security.Claims;

namespace MOODYLIST.Pages
{
    public class PreferitiModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PreferitiModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Playlist> Playlists { get; set; } = new();

        public void OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Playlists = _context.Playlists
                .Where(p => p.UserId == userId)
                .Include(p => p.Songs)
                .ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var playlist = _context.Playlists
                .Include(p => p.Songs)
                .FirstOrDefault(p => p.Id == id && p.UserId == userId);

            if (playlist != null)
            {
                _context.Canzoni.RemoveRange(playlist.Songs);
                _context.Playlists.Remove(playlist);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
