#!/bin/bash
indomainlm=~/data/brown/brown/processed/cr/counts/foo.lm
outdomainlm=~/data/brown/brown/processed/counts/foo.lm
candidatedir=~/data/brown/brown/processed
procs=2
echo Computing perplexity scores in domain
ls -1 $candidatedir/*proc | xargs -P $procs -I REPL sh -c "~/src/srilm/lm/bin/i686-m64/ngram -lm $indomainlm -ppl REPL -debug 1 | grep ppl= | sed -e 's/.*ppl= \([0-9\.]\+\).*/\1/; $ d' > REPL.indomainppl" 

echo Computing perplexity scores out of domain
ls -1 $candidatedir/*proc | xargs -P $procs -I REPL sh -c "~/src/srilm/lm/bin/i686-m64/ngram -lm counts/foo.lm -ppl REPL -debug 1 | grep ppl= | sed -e 's/.*ppl= \([0-9\.]\+\).*/\1/; $ d' > REPL.outdomainppl" 

echo Finding PPL diffs
ls -1 $candidatedir/*proc | xargs -P $procs -I REPL sh -c "paste -d - REPL.indomainppl REPL.outdomainppl | bc > REPL.ppldiff" 

echo Joining and sorting
ls -1 $candidatedir/*.proc | xargs -P $procs -I REPL sh -c "paste REPL.ppldiff REPL | sort -n -k 1 > REPL.srt" 

echo Merging sorted
sort --parallel=$procs -n -m -k 1 $candidatedir/*.srt > all.srtmerge




