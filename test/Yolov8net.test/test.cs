using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

// Create new Yolov8 predictor, specifying the model (in ONNX format)
// If you are using a custom trained model, you can provide an array of labels. Otherwise, the standard Coco labels are used.
using var yolo = YoloV8Predictor.Create("./assets/fpt.onnx", new string[] {"CartonBox", "MilkBox", "PCB1", "PCB2"});

// Provide an input image.  Image will be resized to model input if needed.
using var image = Image.FromFile("Assets/test.png");
var predictions = yolo.Predict(image);

// Draw your boxes
Bitmap bmp = new Bitmap(image, image.Width, image.Height);
using var graphics = Graphics.FromImage(bmp);
foreach (var pred in predictions)
{
    var originalImageHeight = image.Height;
    var originalImageWidth = image.Width;

    var x = Math.Max(pred.Rectangle.X, 0);
    var y = Math.Max(pred.Rectangle.Y, 0);
    var width = Math.Min(originalImageWidth - x, pred.Rectangle.Width);
    var height = Math.Min(originalImageHeight - y, pred.Rectangle.Height);

    ////////////////////////////////////////////////////////////////////////////////////////////
    // *** Note that the output is already scaled to the original image height and width. ***
    ////////////////////////////////////////////////////////////////////////////////////////////

    // Bounding Box Text
    string text = $"{pred.Label.Name} [{pred.Score}]";

    
    graphics.CompositingQuality = CompositingQuality.HighQuality;
    graphics.SmoothingMode = SmoothingMode.HighQuality;
    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

    // Define Text Options
    Font drawFont = new Font("consolas", 11, FontStyle.Regular);
    SizeF size = graphics.MeasureString(text, drawFont);
    SolidBrush fontBrush = new SolidBrush(Color.Black);
    Point atPoint = new Point((int)x, (int)y - (int)size.Height - 1);

    // Define BoundingBox options
    Pen pen = new Pen(Color.Yellow, 2.0f);
    SolidBrush colorBrush = new SolidBrush(Color.Yellow);

    // Draw text on image 
    graphics.FillRectangle(colorBrush, (int)x, (int)(y - size.Height - 1), (int)size.Width, (int)size.Height);
    graphics.DrawString(text, drawFont, fontBrush, atPoint);

    // Draw bounding box on image
    graphics.DrawRectangle(pen, x, y, width, height);
}
bmp.Save("./results/result.jpg", ImageFormat.Jpeg);
