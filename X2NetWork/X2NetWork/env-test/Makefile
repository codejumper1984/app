INCPAHT:=""
CPPFLAGS:="-DEPOLL_CONTROL"
srcfile=$(wildcard *.cpp)
objs=$(subst cpp,a,$(srcfile))

all:$(objs) ../lib/libx2network.a
	@echo "finishe"

$(objs):%.a:%.cpp
	g++ $(CPPFLAGS) -g $^  -o $@ -I "../../gtest/include" -L../lib -lx2network -lpthread
clean:
	rm -f $(objs)
	rm -rf lib

