
#I "../Journal/bin/Debug"
#r "FSharp.Core.dll"
#r "FParsec.dll"
#r "FParsecCS.dll"
#r "Journal.dll"

open WealthPulse
open System.IO

//let ledgerFilePath = @"C:\Users\Mark\Nexus\Documents\finances\ledger\ledger.dat"
//
//let journal = Parser.parseJournalFile ledgerFilePath System.Text.Encoding.ASCII

// parse a journal file with import statements [AJ]
let indexFilePath = @"/Users/andrew/code/ledger/data/index.ledger"

let parseWithIncludes filePath = 
    let rootJournalFile = new FileInfo(filePath)
    let importRegex = System.Text.RegularExpressions.Regex(@"^include (.*)")
    let outStream = new MemoryStream()
    let sw = new StreamWriter(outStream)
    use fs = File.OpenRead(indexFilePath)
    use reader = new StreamReader(fs)
    while reader.Peek() >= 0 do
        let line = reader.ReadLine()
        let m = importRegex.Match(line)
        if m.Success then
            let importFilePath = Path.Combine(rootJournalFile.DirectoryName, m.Groups.[1].Value)
            use importFile = File.OpenRead(importFilePath)
            use importFileReader = new StreamReader(importFile)
            sw.Write(importFileReader.ReadToEnd()) // todo recurse here for further imports?
        else
            sw.WriteLine(line)
    Parser.parseJournalStream outStream System.Text.Encoding.ASCII

let j = parseWithIncludes indexFilePath
