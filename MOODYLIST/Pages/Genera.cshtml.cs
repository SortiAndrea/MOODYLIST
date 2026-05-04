using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOODYLIST.Data;
using MOODYLIST.Model;
using MOODYLIST.Services;
using System.Security.Claims;

namespace MOODYLIST.Pages
{
    public class GeneraModel : PageModel
    {
        private readonly AIChat _gemini;
        private readonly ApplicationDbContext _context;

        public GeneraModel(AIChat gemini, ApplicationDbContext context)
        {
            _gemini = gemini;
            _context = context;
        }

        [BindProperty] public string UserInput { get; set; } = string.Empty;
        [BindProperty] public List<Canzone> Songs { get; set; } = new();
        public bool Saved { get; set; } = false;

        public async Task<IActionResult> OnPostAsync()
        {
            Songs = await _gemini.GeneratePlaylistAsync(UserInput);
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!User.Identity!.IsAuthenticated)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            if (Songs == null || Songs.Count == 0)
                return Page();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nome = UserInput.Length > 30 ? UserInput[..30] + "…" : UserInput;

            var playlist = new Playlist
            {
                Name = $"Playlist – {nome}",
                Mood = UserInput,
                UserId = userId!,
                CreatedAt = DateTime.Now
            };

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            foreach (var song in Songs)
            {
                _context.Canzoni.Add(new Canzone
                {
                    Title = song.Title,
                    Artist = song.Artist,
                    PlaylistId = playlist.Id
                });
            }

            await _context.SaveChangesAsync();

            Saved = true;
            return Page();
        }
    }
}
