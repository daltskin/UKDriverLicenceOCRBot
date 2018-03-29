namespace UKDriverLicenceOCRBot
{
    using System.Collections.Generic;

    public class OCRResponse
    {
        public string Language { get; set; }
        public string Orientation { get; set; }
        public double TextAngle { get; set; }
        public List<Region> Regions { get; set; }
    }

    public class Region
    {
        public string BoundingBox { get; set; }
        public List<Line> Lines { get; set; }
    }

    public class Line
    {
        public string BoundingBox { get; set; }
        public List<Word> Words { get; set; }
    }

    public class Word
    {
        public string BoundingBox { get; set; }
        public string Text { get; set; }
    }
}