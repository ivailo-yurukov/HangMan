using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HangMan
{
    [Serializable]
    public class GameSaveInfo
    {
        public string Word { get; set; }
        public char[] RevealedLetters { get; set; }
        public int WrongLetterCounter { get; set; }
        public StringBuilder WrongLettersResult { get; set; }
    }
}
