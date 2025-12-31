using Wacton.Unicolour;

var max = (
        from r in Enumerable.Range(0, 256)
        from g in Enumerable.Range(0, 256)
        from b in Enumerable.Range(0, 256)
        select (r, g, b)).AsParallel()
    .Aggregate(
        static () => Enumerable.Repeat((double.MinValue, (0, 0, 0)), 14).ToArray(),
        static (localMax, rgb) => {
            Unicolour color = new(ColourSpace.Rgb, rgb.r / 255.0, rgb.g / 255.0, rgb.b / 255.0);
            Update(ref localMax[0], (color.Hsb.S, rgb));
            Update(ref localMax[1], (color.Hsl.S, rgb));
            Update(ref localMax[2], (color.Hsi.S, rgb));
            Update(ref localMax[3], (color.Lchab.C, rgb));
            Update(ref localMax[4], (color.Lchuv.C, rgb));
            Update(ref localMax[5], (color.Hsluv.S, rgb));
            Update(ref localMax[6], (color.Hpluv.S, rgb));
            Update(ref localMax[7], (color.Tsl.S, rgb));
            Update(ref localMax[8], (color.Jzczhz.C, rgb));
            Update(ref localMax[9], (color.Oklch.C, rgb));
            Update(ref localMax[10], (color.Okhsv.S, rgb));
            Update(ref localMax[11], (color.Okhsl.S, rgb));
            Update(ref localMax[12], (color.Oklrch.C, rgb));
            Update(ref localMax[13], (color.Hct.C, rgb));
            return localMax;
        },
        static (globalMax, localMax) => {
            for (var i = 0; i < 14; i++)
                Update(ref globalMax[i], localMax[i]);
            return globalMax;
        },
        static max => max);

foreach (var (val, (r, g, b)) in max)
    Console.WriteLine($"({r}, {g}, {b})\t{val}");
return;

static void Update(
    ref (double val, (int, int, int) rgb) max,
    (double val, (int, int, int) rgb) cur) {
    if (cur.val > max.val)
        max = cur;
}

/* 结果：
 Hsb
 Hsl
 Hsi
 Lchab
 Lchuv
 Hsluv
 Hpluv
 Tsl
 Jzczhz
 Oklch
 Okhsv
 Okhsl
 Oklrch
 Hct
 */
