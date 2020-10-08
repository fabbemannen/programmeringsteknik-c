using CommandLine;
using Imageflow.Fluent;
using System.IO;
using System.Runtime.CompilerServices;

namespace Resizer
{
    class Options
    {
        [Option('i', "input", Required = true, HelpText = "Path to input file.")]
        public string Input { get; set; }

        [Option('w', "width", Required = false, HelpText = "Width of output image.")]
        public uint? Width { get; set; }

        [Option('h', "height", Required = false, HelpText = "Height of output image.")]
        public uint? Height { get; set; }

        [Option('s', "saturation", Required = false, HelpText = "Color saturation of output image.")]
        public float? Saturation { get; set; }

        [Option('b', "brightness", Required = false, HelpText = "Brightness of output image.")]
        public float? Brightness { get; set; }

        [Option('c', "contrast", Required = false, HelpText = "Contrast of output image.")]
        public float? Contrast { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Dessa övningar använder imageflow
            // https://github.com/imazen/imageflow-dotnet#examples
            // (alla beroenden är installerade i projektet redan)

            // ImageJob.Decode med en System.IO.Stream som parameter laddar in en bild.
            // BuildNode.EncodeToStream (via method chain) kan användas för att skriva till fil

            // På grund av att imageflow är anpassat att köras på server, med kö-hantering,
            // behöver Finish().InProcessAsync() kallas för att beordra avslut på körningen.
            // InProcessAsync() är en asynkron metod och behöver inväntas, 
            // detta kan göras genom att lägga till .Wait(), annars avslutas programmet för tidigt.

            // Options-objektet behöver skapas från args
            // https://github.com/commandlineparser/commandline#quick-start-examples


            // 1. Skala om en bild beroende på angiven breddparameter
            // 2. Lägg till en höjdparameter och skala om beroende på dessa.
            // 3. Lägg till ett skärpefilter om bildens storlek minskas.
            // 4. Lägg till parametrar för färgmättnad, ljusstyrka och kontrast.

            Parser.Default.ParseArguments<Options>(args)
                          .WithParsed<Options>(Run);
        }

        static void Run(Options options)
        {
            using (var stream = File.OpenRead(options.Input))
            {
                using (var outStream = File.OpenWrite(GetOutputFileName(options.Input)))
                {
                    using (var job = new ImageJob())
                    {
                        string resizerCommandLine = string.Format("{0}{1}&mode=crop",
                            options.Width.HasValue ? "width=" + options.Width : string.Empty,
                            options.Height.HasValue ? "&height=" + options.Height : string.Empty
                            );

                        //Defaults in program arguments?
                        float saturation = options.Saturation.HasValue ? (float)options.Saturation : 0.0f;
                        float brightness = options.Brightness.HasValue ? (float)options.Brightness : 0.0f;
                        float contrast = options.Contrast.HasValue ? (float)options.Contrast : 0.0f;

                        job.Decode(stream, false)
                            .ResizerCommands(resizerCommandLine)
                            .SaturationSrgb(saturation)
                            .BrightnessSrgb(brightness)
                            .ContrastSrgb(contrast)
                            .EncodeToStream(outStream, false, new MozJpegEncoder(100, true))
                            //.Encode(stream.Name ,new MozJpegEncoder(100, true))
                            .Finish()
                            .InProcessAsync()
                            .Wait();

                    }
                }

            }
        }

        static string GetOutputFileName(string path)
        {
            string directory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);

            return $"{directory}/{fileName}-resized{extension}";
        }
    }
}
