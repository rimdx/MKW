cmake_minimum_required(VERSION 3.27)

project(mkw LANGUAGES C CXX)
list(APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_SOURCE_DIR}/build")

find_package(PkgConfig)
include(GNUInstallDirs)

pkg_check_modules(nettle REQUIRED IMPORTED_TARGET nettle)
pkg_check_modules(hogweed REQUIRED IMPORTED_TARGET hogweed)
pkg_check_modules(gmp REQUIRED IMPORTED_TARGET gmp)

add_executable(mkw
    main.c
)
target_link_libraries(mkw PRIVATE
    PkgConfig::nettle
    PkgConfig::hogweed
    PkgConfig::gmp
)

add_executable(app
    app.cpp
)
find_package(wxWidgets REQUIRED COMPONENTS core base)
target_link_libraries(app PRIVATE wxWidgets::wxWidgets)
