CC = g++
CFLAGS = -I ~/src/srilm/lm/src -I ~/src/srilm/include -DNO_TCL -fopenmp -g -O3
LFLAGS = -L ~/src/srilm/lib/i686-m64/

all: ppl_map.cpp
	$(CC) $(CFLAGS) $(LFLAGS) ppl_map.cpp -loolm -lflm -ldstruct -lmisc -lz -lpthread -o pplmapper
