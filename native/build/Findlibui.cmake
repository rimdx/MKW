find_path(LIBUI_INCDIR
    NAMES ui.h
    PATH_SUFFIXES ${CMAKE_INSTALL_INCLUDEDIR}
)

find_library(LIBUI_LIBPATH
    NAMES ui
    PATH_SUFFIXES ${CMAKE_INSTALL_LIBDIR}
)

mark_as_advanced(LIBUI_INCDIR LIBUI_LIBPATH)

include(FindPackageHandleStandardArgs)

find_package_handle_standard_args(libui
    REQUIRED_VARS
        LIBUI_LIBPATH
        LIBUI_INCDIR
)

add_library(libui::ui IMPORTED STATIC)
set_target_properties(libui::ui PROPERTIES
    INTERFACE_INCLUDE_DIRECTORIES ${LIBUI_INCDIR}
    IMPORTED_LOCATION ${LIBUI_LIBPATH}
)
