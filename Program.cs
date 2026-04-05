using System;
using System.Threading;
using System.Threading.Tasks;
using SoundpadConnector;
using SoundpadConnector.XML;
using System.Linq;

namespace SoundpadCoding
{
    class Program
    {
        private static readonly Soundpad Soundpad = new Soundpad();
        private static readonly Random Random = new Random();
        private static int oldNumber = -1;

        public static async Task Main(string[] args)
        {
            await ConnectToSoundpad();

            Console.Write("Getting the number of sounds");
            var countResult = await Soundpad.GetSoundFileCount();
            int maxSounds = Convert.ToInt32(countResult.Value);
            await PrintDots();
            await Task.Delay(500);
            Console.WriteLine($"Found {maxSounds} sound effects! :)");
            await Task.Delay(500);

            Console.Write("Set how often random sounds will be played in seconds: ");
            if (!int.TryParse(Console.ReadLine(), out int delaySeconds) || delaySeconds <= 0)
            {
                Console.WriteLine("Invalid input. Enter an integer greater than 0.");
                return;
            }
            int delay = delaySeconds * 1000;
            Console.WriteLine($"Now playing sounds every {delaySeconds} seconds...");
            await Task.Delay(350);

            await PlayRandomSounds(maxSounds, delay);
        }
        private static async Task ConnectToSoundpad()
        {
            Console.Write("Connecting to Soundpad");
            await PrintDots();
            await Soundpad.ConnectAsync();
            if (Soundpad.ConnectionStatus == ConnectionStatus.Connected)
            {
                Console.WriteLine("Soundpad has connected succesfully!");
            }
            else
            {
                Console.WriteLine("There was an error trying to connect to Soundpad. Please ensure the program is running.");
                await Task.Delay(2500);
                throw new Exception("Quitting...");
            }
            await Task.Delay(500);
        }
        private static async Task PrintDots()
        {
            await Task.Delay(250);
            Console.Write(".");
            await Task.Delay(250);
            Console.Write(".");
            await Task.Delay(250);
            Console.Write(".");
            await Task.Delay(500);
            Console.WriteLine();
        }
        private static async Task PlayRandomSounds(int maxSounds, int delay)
        {
            var soundListResponse = await Soundpad.GetSoundlist();
            var sounds = soundListResponse.Value.Sounds;
            while (true){
                int number;
                do
                {
                    number = Random.Next(1, maxSounds);
                }
                while (number == oldNumber);

                oldNumber = number;

                await Soundpad.PlaySound(number);

                var soundName = sounds[number-1].Title;
                Console.WriteLine($"Now playing sound \"{soundName}\" with index {number}.");
                await Task.Delay(delay);
            }
        }
    }
}