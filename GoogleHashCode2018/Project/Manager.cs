using System;
using System.Collections.Generic;
using System.Text;

using FileParser;
using FileScriber;

namespace Project
{
    public class Manager
    {
        private readonly string _inputPath = "./Samples/";
        private readonly string _outputPath = "./Outputs/";

        private readonly string _inputFileName = "Small.txt";
        private readonly string _outputFileName = null;

        private ParsedFile _file;

        public Manager()
        {
            _outputFileName = "output_" + _inputFileName;
            _inputPath += _inputFileName;
            _outputPath += _outputFileName;
        }

        public void Run()
        {
            LoadData();

            ProcessData();

            PrintData();
        }

        private void LoadData()
        {
            _file = new ParsedFile(_inputPath);

            IParsedLine line = _file.NextLine();
            int n = line.NextElement<int>();
        }

        private void PrintData()
        {
            var file = _outputPath;
            Writer.Write<int>(file, 5);
        }

        private void ProcessData()
        {
            ;
        }
    }
}
