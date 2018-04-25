SRCLANG=de
TGTLANG=en
INDOMAIN_SRC=~/src/domainadapt/testdata/europarl/europarl-v7.de-en.de
INDOMAIN_TGT=~/src/domainadapt/testdata/europarl/europalr-v7.de-en.en
OUTDOMAIN_SRC=~/src/domainadapt/testdata/commoncrawl/commoncrawl.de-en.de
OUTDOMAIN_TGT=~/src/domainadapt/testdata/commoncrawl/commoncrawl.de-en.en
MOSES_SCRIPT_DIR = ~/src/mosesdecoder/scripts/tokenizer
BIGFATLM_SCRIPT_DIR=~/src/bigfatlm
HDFS_ROOT = /domainadapt
HDFS_INDOMAIN_SRCFILE = $(HDFS_ROOT)/indomain_src
HDFS_INDOMAIN_LM = $(HDFS_ROOT)/indomain_src_lm
HDFS_OUTDOMAIN_SRCFILE = $(HDFS_ROOT)/outdomain_src
HADOOP=/usr/local/hadoop/bin/hadoop
ORDER=3

all: indomain_src_lm outdomain_src_lm

outdomain_src_lm: outdomain_tok_src
	$(BIGFATLM_SCRIPT_DIR)/build-lm.sh $(ORDER) outdomain_tok_src corpus-3g-lm /home/anthonyaue/src/domainadapt/recipe/outdomain_src_lm --noFilter 

indomain_src_lm: indomain_tok_src
	#Always recopy to hdfs for now
	#HDFS doesn't seem to work correctly.
	#- $(HADOOP) dfs -rm -R $(HDFS_ROOT)
	#$(HADOOP) dfs -mkdir -p $(HDFS_ROOT)
	#$(HADOOP) dfs -put /home/anthonyaue/src/domainadapt/recipe/indomain_tok_src $(HDFS_INDOMAIN_SRCFILE)
	$(BIGFATLM_SCRIPT_DIR)/build-lm.sh $(ORDER) indomain_tok_src corpus-3g-lm /home/anthonyaue/src/domainadapt/recipe/indomain_src_lm --noFilter 
	#$(HADOOP) dfs -get $(HDFS_INDOMAIN_LM) /home/anthonyaue/src/domainadapt/recipe/indomain_src_lm

indomain_tok_src: $(INDOMAIN_SRC)
	cat $(INDOMAIN_SRC) | $(MOSES_SCRIPT_DIR)/lowercase.perl | $(MOSES_SCRIPT_DIR)/tokenizer.perl -l $(SRCLANG) > indomain_tok_src
	
outdomain_tok_src: $(OUTMAIN_SRC)
	cat $(OUTDOMAIN_SRC) | $(MOSES_SCRIPT_DIR)/lowercase.perl | $(MOSES_SCRIPT_DIR)/tokenizer.perl -l $(SRCLANG) > outdomain_tok_src
	




