#include "Ngram.h"
#include "LM.h"
#include "File.h"
#include <iostream>
#include <string.h>

using namespace std;

void InitVocab(Vocab* pInitMe)
{
    pInitMe->unkIsWord() = false;
    pInitMe->toLower() = false;
}

void InitLM(Ngram* initMe, const char* lmFile)
{
    File file(lmFile, "r");

    if (!initMe->read(file, false)) {
        cerr << "Error opening file " << lmFile << std::endl;
    }
}

float GetSentencePPL(char* line, Vocab* pVocab, Ngram* pLM)
{

    VocabString sentence[maxWordsPerLine + 1];
    unsigned int numWords =
        pVocab->parseWords(line, sentence, maxWordsPerLine + 1);

    if (numWords == maxWordsPerLine + 1) 
    {
        return 10000;
    }
    else
    {
        TextStats sentenceStats;
        FloatCount weight;
        VocabString *sentenceStart;

        weight = 1.0;
        sentenceStart = sentence;


        LogP prob = pLM->sentenceProb(sentenceStart, sentenceStats);

        double denom = sentenceStats.numWords - sentenceStats.numOOVs - sentenceStats.zeroProbs + sentenceStats.numSentences;

        return LogPtoPPL(sentenceStats.prob / denom);

    }
}

int main(int argc, char** argv)
{
    if(argc != 4)
    {
        cerr << "Usage: ppl_map INDOMAINLM OUTOFDOMAINLM FILE";
        return -1;
    }
    Vocab* pInDomainVocab = new Vocab;
    Vocab* pOutDomainVocab = new Vocab;
    InitVocab(pInDomainVocab);
    InitVocab(pOutDomainVocab);
    unsigned int order = 3;
    Ngram* pInDomainLM = new Ngram(*pInDomainVocab, order);
    Ngram* pOutDomainLM = new Ngram(*pOutDomainVocab, order);
    

    InitLM(pInDomainLM, argv[1]);
    InitLM(pOutDomainLM, argv[2]);
    //start
    //
    char *line;
    char lineCpy[8092];

    File sntFile(argv[3], "r");
    while ((line = sntFile.getline())) 
    {
        strcpy(lineCpy, line);
        const char* delim = "\t";
        char* pId = strtok(lineCpy, delim);
        if(pId == NULL) continue;
        char* pLinePart = strtok(lineCpy, delim);
        if(pLinePart == NULL) continue;
        
        float indom = GetSentencePPL(pLinePart, pInDomainVocab, pInDomainLM);
        float outdom = GetSentencePPL(pLinePart, pOutDomainVocab, pOutDomainLM);
        float diff = indom - outdom;
        cout << pId << '\t' << diff << endl;        
    }

}
