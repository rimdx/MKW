cmake_minimum_required(VERSION 3.12)

project(mkw LANGUAGES C)

find_package(PkgConfig)

pkg_check_modules(ncurses REQUIRED IMPORTED_TARGET ncurses)
pkg_check_modules(nettle REQUIRED IMPORTED_TARGET nettle)
pkg_check_modules(hogweed REQUIRED IMPORTED_TARGET hogweed)
pkg_check_modules(gmp REQUIRED IMPORTED_TARGET gmp)

add_executable(mkw
    main.c
)
target_link_libraries(mkw PRIVATE
    PkgConfig::ncurses
    PkgConfig::nettle
    PkgConfig::hogweed
    PkgConfig::gmp
)
