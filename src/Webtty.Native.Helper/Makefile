build:
	mkdir -p bin
	cd bin; \
	clang -Wall -o const-helper ../main.c -g -v

clean:
	rm -rf ./bin

run: build
	./bin/const-helper
