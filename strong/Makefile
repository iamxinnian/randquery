DIR_OBJ=./obj
DIR_SRC=./src
DIR_INC=./include
SRC=$(wildcard ${DIR_SRC}/*.cpp ./*.cpp)
OBJ=$(patsubst %.cpp,${DIR_OBJ}/%.o,$(notdir $(SRC)))

TARGET=gateway_app

CXX =/usr/local/arm/arm-linux-gnueabi-4.9/bin/arm-linux-gnueabi-g++
CPPFLAGS = -lpthread -lsqlite3 -I$(DIR_INC) -L./lib/

$(TARGET):$(OBJ)
	$(CXX) $(OBJ) $(CPPFLAGS) -o $@
$(DIR_OBJ)/%.o:$(DIR_SRC)/%.cpp
	$(CXX) $(CPPFLAGS) -c $< -o $@
$(DIR_OBJ)/%.o:./%.cpp
	$(CXX) $(CPPFLAGS) -c $< -o $@

.PHONY:clean
clean:
	-find $(DIR_OBJ) -name *.o -exec rm -rf {} \;
	rm $(TARGET)
