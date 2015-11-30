INCPAHT:=""
CPPFLAGS:="-DEPOLL_CONTROL"
srcfile=$(wildcard src/*.cpp)
objs=$(subst cpp,o,$(srcfile))

all:$(objs)
	mkdir -p lib
	ar rcs ./lib/libx2network.a $(objs)
	rm -f $(objs)
$(objs):%.o:%.cpp
	g++ $(CPPFLAGS) -g $^ -c -o $@
clean:
	rm -f $(objs)
	rm -rf lib

