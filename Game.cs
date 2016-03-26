using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HangMan
{
    public class Game
    {
        public string word;
        public char[] RevealedLetters { get; private set; }
        public int WrongLetterCounter { get; private set; }
        public StringBuilder WrongLettersResult { get; private set; }

        public Game()
        {
            word = GetRandomWord();
            RevealedLetters = new char[word.Length];
            WrongLettersResult = new StringBuilder();
            for (int i = 0; i < RevealedLetters.Length; i++)
            {
                RevealedLetters[i] = '-';
            }
        }

        public Game(GameSaveInfo saveInfo)
        {
            word = saveInfo.Word;
            RevealedLetters = saveInfo.RevealedLetters;
            WrongLetterCounter = saveInfo.WrongLetterCounter;
            WrongLettersResult = saveInfo.WrongLettersResult;

        }

        public bool IsRightLetter(char guessLetter)
        {
            bool isRight = false;

            for (int i = 0; i < word.Length; i++)
            {
                if (guessLetter.ToString().ToLower() == word[i].ToString().ToLower())
                {
                    RevealedLetters[i] = guessLetter;
                    isRight = true;
                }
            }
            if (!isRight)
            {
                WrongLetterCounter++;
                WrongLettersResult.Append(guessLetter);
                WrongLettersResult.Append(" , ");
            }                               
            return isRight;
        }

        public bool IsWon()
        {
            bool isEnd = true;

            for (int i = 0; i < word.Length; i++)
            {
                if (RevealedLetters[i] == '-')
                {
                    isEnd = false;
                    break;
                }
            }
            return isEnd;
        }


        private string GetRandomWord()
        {
            string[] words = new string[] { "pop", "dog", "paint", "hangmannnnnnn", "bike" };
            Random random = new Random();
            int randomWord = random.Next(0, words.Length);
            return words[randomWord].ToUpper();
        }
    }
}
