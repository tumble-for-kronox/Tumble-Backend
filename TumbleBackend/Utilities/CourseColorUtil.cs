using KronoxAPI.Model.Scheduling;
using WebAPIModels.Extensions;
using WebAPIModels;

namespace TumbleBackend.Utilities;

public static class CourseColorUtil
{
    // Colors from website: https://htmlcolorcodes.com/colors/

    private static readonly List<string> _colors = new()
    {
        // Shades of blue
        "#00FFFF", // Aqua
        "#89CFF0", // Baby Blue
        "#0000FF", // Blue
        "#7393B3", // Blue Gray
        "#088F8F", // Blue Green
        "#0096FF", // Bright Blue
        "#1434A4", // Egyptian Blue
        "#00A36C", // Jade
        "#5D3FD3", // Iris
        "#191970", // Midnight Blue
        "#CCCCFF", // Periwinkle
        "#96DED1", // Robin Egg Blue
        "#9FE2BF", // Seafoam Blue
        "#008080", // Teal
        
        // Shades of brown
        "#E1C16E", // Brass
        "#CD7F32", // Bronze
        "#DAA06D", // Buff
        "#800020", // Burgundy
        "#E97451", // Burnt Sienna
        "#6E260E", // Burnt Umber
        "#7B3F00", // Chocolate
        "#6F4E37", // Coffee
        "#8B0000", // Dark Red
        "#E5AA70", // Fawn
        "#F0E68C", // Khaki
        "#C04000", // Mahogany
        "#800000", // Maroon
        
        // Shades of green
        "#AFE1AF", // Celadon
        "#50C878", // Emerald green
        "#228B22", // Forest green
        "#32CD32", // Lime green
        "#478778", // Lincoln green
        "#93C572", // Pistachio
        
        // Shades of orange
        "#FFBF00", // Amber
        "#FFAC1C", // Bright orange
        "#F88379", // Coral pink
        "#FF7F50", // Coral
        "#E49B0F", // Gamboge
        "#F4BB44", // Mango
        "#FF5F1F", // Neon orange
        "#EC5800", // Persimmon
        "#FA8072", // Salmon
        
        // Shades of pink
        "#9F2B68", // Amaranth
        "#DE3163", // Cerise
        "#AA336A", // Dark pink
        "#FF69B4", // Hot pink
        "#770737", // Mulberry
        "#DA70D6", // Orchid
        "#800080", // Purple
        "#E30B5C", // Raspberry
        "#F33A6A", // Rose
        "#D8BFD8", // Thistle
        
        // Shades of purple
        "#CF9FFF", // Light violet
        "#51414F", // Quartz
        "#7F00FF", // Violet
        
        // Shades of red
        "#880808", // Blood red
        "#AA4A44", // Brick red
        "#D22B2B", // Cadmium red
        "#FF3131", // Neon red
        "#E0115F", // Ruby red
        "#FF2400", // Scarlet
        "#A42A04", // Venetian red
        "#E34234", // Vermillion
        
        // Shades of yellow
        "#FFEA00", // Bright yellow
        "#FDDA0D", // Cadmium yellow
        "#FAD5A5", // Desert
        "#FFD700", // Gold
        "#FADA5E", // Navel yellow
    };

    /// <summary>
    /// Pick a random set of colors of size <paramref name="count"/>.
    /// </summary>
    /// <param name="count"></param>
    /// <returns><see cref="Array"/> with <paramref name="count"/> number of hex color codes.</returns>
    public static string[] GetScheduleColors(int count)
    {
        HashSet<string> usedColors = new();
        string[] colors = new string[count];
        for (int i = 0; i < count; i++)
        {
            string color = GetSingleRandColor();
            while (usedColors.Contains(color))
                color = GetSingleRandColor();

            usedColors.Add(color);
            colors[i] = color;
        }

        return colors;
    }

    public static string GetSingleRandColor()
    {
        Random rand = new();
        return _colors[rand.Next(_colors.Count)];
    }
}
