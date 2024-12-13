// EXAMPLE TO GET IMAGE LINKS
// https://images.igdb.com/igdb/image/upload/t_screenshot_med/sc78fa.webp
// https://images.igdb.com/igdb/image/upload/{ResolutionFolder}/sc78fa.webp


// RESOLUTION FOLDERS BELOW:
// t_thumb	Thumbnail	90x90
// t_cover_small	Small cover image	90x128
// t_cover_big	Large cover image	264x374
// t_logo_med	Medium logo	284x160
// t_screenshot_med	Medium screenshot	569x320   [THIS ONE FOR INITIAL IMAGES maybe...]
// t_screenshot_big	Big screenshot	889x500
// t_screenshot_huge	Huge screenshot	1280x720
// t_720p	720p resolution	1280x720
// t_1080p	1080p resolution	1920x1080
// t_micro	Very small image	35x35
// t_steam	Steam-like image	512x512
// t_original	Original image size (no resizing)	Full resolution  [THIS ONE FOR CLICKED IMAGES]


namespace MyApp.Models.Igdb
{

    //Igdb theme data from api
    public class ScreenShot
    {
        public string CoverSizeURL { get; set; } //"https://images.igdb.com/igdb/image/upload/t_screenshot_med/*.webp"
        public string OriginalSizeURL { get; set; } //"https://images.igdb.com/igdb/image/upload/t_original/*.webp"

        public ScreenShot() { }
    }
}