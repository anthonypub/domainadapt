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


To figure out where the original line came from, we need to know the original file name and the original line number.

These can be encoded in two 32-bit ints or so, so having 64-bit id's ought to do it. Then we just need a table of (file id -> file location), and we have what we need.

That's in mapper, invoke test like this: dotnet bin/Debug/netcoreapp2.0/mapper.dll

So now the whole process, for a mono adaptation, would be to do this:

Given an in-domain and out-of-domain corpus:

- Run mapper on corpus so we have a single file with all of the data (one for in-domain LM, one for out of domain).
- Upload files to hdfs
- Build in-domain and out-of-domain language models using Jon's toolkit (modify to ignore sentence id)
- Run hadoop job over out-of-domain data using perplexity mapper with identity reducer and appropriate sort/partiion config to get out sorted file
- Run mapper again to get original sentences out


To start hadoop name node:
sbin/start-dfs.sh

To view:
http://localhost:9870/


Which language to encode this whole thing in?

Bash?
Python?
C#?
Make?

There's enough going on that I don't really trust myself to do this in bash, partly because I suck at it and partly because it's not well suited to the task.

Most of the data processing is done by outside programs - perl for preprocessing, hadoop for lm building, hadoop for data selection and sorting, so that's not really an issue. 

I'm kind of tempted to use Make. 



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















