cmake_minimum_required(VERSION 3.27)

project(mkw LANGUAGES C CXX)
list(APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_SOURCE_DIR}/build")

find_package(PkgConfig)
include(GNUInstallDirs)

pkg_check_modules(nettle REQUIRED IMPORTED_TARGET nettle)
pkg_check_modules(hogweed REQUIRED IMPORTED_TARGET hogweed)
pkg_check_modules(gmp REQUIRED IMPORTED_TARGET gmp)

add_library(mkw_core
    mkw_core/aes.c
    mkw_core/armour.c
    mkw_core/cfb.c
    mkw_core/memory.c
    mkw_core/mkw_crypto.c
    mkw_core/mkw_pgp.c
    mkw_core/mkw_storage.c
    mkw_core/packet.c
    mkw_core/s2k.c
)
target_link_libraries(mkw_core PRIVATE
    PkgConfig::nettle
    PkgConfig::hogweed
    PkgConfig::gmp
)
target_include_directories(mkw_core PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}/mkw_core)

add_executable(mkw
    main.c
)
target_link_libraries(mkw PRIVATE mkw_core)

add_executable(app
    app.cpp
)
find_package(wxWidgets REQUIRED COMPONENTS core base)
target_link_libraries(app PRIVATE wxWidgets::wxWidgets)
