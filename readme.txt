Lightweight moore - lewis data selection

Required:

in-domain mono or bilingual data
out-of-domain mono or bilingual data

Hadoop install

Jon Clark's bifatlm to build language models (https://github.com/jhclark/bigfatlm)
Perplexity reducer using srilm  (ppl_map.cpp)

Things work for monolingual data, but how to do bilingual data?

We need to be able to join source + target.

Assuming all of the LM's can fit in memory, we could just pass the entire source + target conglomerate to the mapper, along with both LM's, and it could compute the entire corpus.

If we can only fit source or target LM's into memory, the way to do it would be to add identical IDs to aligned source + target sentences. One mapper would do the source side, returning ID\tScore tuples, the other would do the target side, and then we reduce the id's by summing them a la Axelrod.

Then last we would need a component to map the resulting stream of Score\tId back to the sentences for corpus selection.  




===== Old steps from cmd line versio =====

Steps: 

(1) run ngram counts on inputs for in-domain and out-of-domain:

for proc in  *.proc; do ~/src/srilm/lm/bin/i686-m64/ngram-count -sort -order 3 -text $proc -write counts/$proc.gz & wait; done

(2) ditto for out of domain

(3) merge ngram counts
~/src/srilm/lm/bin/i686-m64/ngram-merge -write counts/merged.gz `ls counts/*.gz`

(4) build language models
~/src/srilm/lm/bin/i686-m64/ngram-count -read merged.gz -order 3 lm foo.lm

(5) Get PPL scores
#Score the sentences with in-domain, then pull out the perplexity scores ($ d removes the last (summary) line):

for proc in *.proc ; do ~/src/srilm/lm/bin/i686-m64/ngram -lm counts/foo.lm -ppl $proc -debug 1 | grep ppl= | sed -e 's/.*ppl= \([0-9\.]\+\).*/\1/; $ d' > $proc.indomainppl ; done 

#do the same with out of domain
for proc in *.proc ; do ~/src/srilm/lm/bin/i686-m64/ngram -lm ../counts/foo.lm -ppl $proc -debug 1 | grep ppl= | sed -e 's/.*ppl= \([0-9\.]\+\).*/\1/; $ d' > $proc.outdomainppl ; done 

#Do the same with both language models, then do the subtraction using bc
for proc in *.proc ; do paste -d - $proc.indomainppl $proc.outdomainppl | bc > $proc.ppldiff; done 

#Join with sentence file and sort

for proc in *.proc ; do paste $proc.ppldiff $proc | sort -n -k 1  > $proc.srt ; done 

#Now merge sort
sort -n -m -k 1 *.srt > all.srtmerge















