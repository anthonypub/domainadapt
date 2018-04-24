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

all: indomain_src_lm outdomain_tok_src

indomain_src_lm: indomain_tok_src
	#Always recopy to hdfs for now
	$(HADOOP) dfs -rm -R $(HDFS_ROOT)
	$(HADOOP) dfs -mkdir -p $(HDFS_ROOT)
	$(HADOOP) dfs -cp indomain_tok_src $(HDFS_INDOMAIN_SRCFILE)
	$(BIGFATLM_SCRIPT_DIR)/build-lm.sh $(ORDER) $(HDFS_INDOMAIN_SRCFILE) $(HDFS_INDOMAIN_LM) 
	$(HADOOP) dfs -cp $(HDFS_INDOMAIN_LM) indomain_src_lm

indomain_tok_src: $(INDOMAIN_SRC)
	cat $(INDOMAIN_SRC) | $(MOSES_SCRIPT_DIR)/lowercase.perl | $(MOSES_SCRIPT_DIR)/tokenizer.perl -l $(SRCLANG) > indomain_tok_src
	
outdomain_tok_src: $(OUTMAIN_SRC)
	cat $(OUTDOMAIN_SRC) | $(MOSES_SCRIPT_DIR)/lowercase.perl | $(MOSES_SCRIPT_DIR)/tokenizer.perl -l $(SRCLANG) > outdomain_tok_src
	




