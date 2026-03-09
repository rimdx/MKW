#include <ctype.h> /* for isdigit */

#include "mkw.h"
#include "mkw_pgp.h"

/*
 * ----THE FORMAT-----
 *
 * The payload is a key-value storage. The goal is to store password
 * properties. Those may include something like the password itself, username,
 * title, and many more. All of these properties are encrypted in a single
 * entry with a dedicated session key. This is a bit out of scope of this file.
 *
 * We assume that the key is a valid string. Most probably, one of predifined.
 * Only ASCII characters are allowed. No hidden symbols are allowed. No
 * linebreaks are allowed. Perhaps even whitespaces are banned two. The key is
 * safe to be NULL-terminated C-string.
 *
 * The content on the other hand, may include ANY arbitrary data from a user
 * (including \0's). We might need to deal with contents in binray if it's
 * something like an image lets say that needs to be stored. That's why we
 * wanna store some sort of size hint.
 *
 * The first version does the following; It is based on Subversion's key-value
 * format which is used in dump-files and in the deepths of FSFS database. The
 * original format looks like this:
 *
 * [[[
 * K 6
 * keyname
 * V 11
 * abc
 * def
 * 123
 * ]]]
 *
 * Where K and V are specifiers, what follows after is a ASCII decimal number
 * representing length of data, and then follows the data blobs themselves. The
 * headers with length are newline terminated. Also it allows END string
 * literal instead of a new key that represent the end of a hash. This is not
 * needed in our case, but could be useful if it was an issue.
 *
 * However, it's a bit of an overkill for us to allow arbitrary keys, with any
 * symbols. We would be fine with just newline terminated. What it is right
 * just looks verbose. That's why we do the following:
 *
 * [[[
 * K keyname
 * V 11
 * abc
 * def
 * 123
 * ]]]
 *
 * ...also similar but with a leaked real-world timofei's creadentials from a
 * very important website of his use...
 *
 * [[[
 * K mkw:username
 * V 4
 * tima
 * K mkw:password
 * V 9
 * secret123
 * ]]]
 *
 * ...it's taken from the test suite if that matters...
 *
 * This is basically the same exact thing except key is terminated with a
 * newline (\n). 
 *
 * The main advantages of this format is that it's not as verbose as
 * Subversion's due to simlified keys. At the same time it allows arbitrary
 * data in the contents without any escaping. The only issue is that it's hard
 * and potentially dangerous to parse/serialize decimals. Also there is a
 * possibility to expose allocator to depend on user's data. i.e. allocating
 * 2GBs of memory just because it's stated in the length field. We'd really love
 * to avoid such issues. Current safety measures include not allowing to eat
 * more of body than data available which sounds fair.
 *
 * The entries are sorted by key in asceding order with comparasion done by
 * ASCII value. However, clients should not assume that it's sorted.
 *
 * Users should follow the following conventions when constructing the hashes:
 *
 * - Use Unix-style newline terminators. If it's a multiline field, it MUST be
 *   converted unless it's binray.
 *
 * - All strings (in content) are UTF-8 enocoded unless it's a binary field.
 *
 * - All keys are in ASCII. Only printable characters are allowed. Whitespace,
 *   newlines, tabs -- everything like this is banned.
 *
 * - Keys may contain namespaces. The namespaces are separated by a semicolon
 *   (:). Namespaces can be nested. Keys that start with 'mkw' are reserved for
 *   official clients. Examples: 'mkw:password', 'mkw:title',
 *   'ContosoClientXYZ:StaffID'.
 *
 * - Value strings should not include NULL-terminator. It should never occur in
 *   payload unless it's a part of binary blob.
 */

static void
memrev(char *str, size_t len)
{
    if (len > 0) {
        char *s = str;
        char *e = str + len - 1;

        for (; s < e; s++, e--) {
            char tmp = *s;
            *s = *e;
            *e = *s;
        }
    }
}

static void
write_length(mkw_membuf_t *out, uint64_t len)
{
    char buf[32] = { 0 };
    size_t i;

    for (i = 0; len; i++) {
        assert(i < sizeof(buf));
        buf[i] = '0' + (len % 10);
        len /= 10;
    }

    memrev(buf, i);
    mkw_membuf_write_str(out, (uint8_t *)buf, i);
}

static mkw_error_t
read_length(mkw_memreader_t *reader, uint64_t *len)
{
    *len = 0;

    while (1) {
        uint8_t cur;
        MKW_ERR(mkw_memreader_read_uint8(reader, &cur));

        if (cur == '\n') {
            break;
        } else if (isdigit(cur)) {
            *len = (*len * 10) + (cur - '0');

            /* check for a potential overflow. (the len is 64-bit integer) */
            if (*len > UINT32_MAX) {
                return MKW_ERROR_PAYLOAD_BAD_LENGTH;
            }
        } else {
            return MKW_ERROR_PAYLOAD_BAD_LENGTH;
        }
    }

    return MKW_ERROR_NONE;
}

#define KEY_PREFIX "K "
#define VAL_PREFIX "V "

void
mkw_payload_write(mkw_membuf_t *out,
                  mkw_payload_entry_t *items[],
                  size_t count)
{
    size_t i;

    for (i = 0; i < count; i++) {
        const mkw_payload_entry_t *entry = items[i];

        mkw_membuf_write_cstr(out, KEY_PREFIX);
        mkw_membuf_write_cstr(out, entry->key);
        mkw_membuf_write_cstr(out, "\n");

        mkw_membuf_write_cstr(out, VAL_PREFIX);
        write_length(out, entry->content->size);
        mkw_membuf_write_cstr(out, "\n");
        mkw_membuf_write_str(out, entry->content->data, entry->content->size);
        mkw_membuf_write_cstr(out, "\n");
    }
}

mkw_error_t
mkw_payload_read(mkw_memreader_t *reader, mkw_vector_t *items,
                 mkw_pool_t *pool)
{
    size_t i;
    mkw_membuf_t *key = mkw_membuf_create_empty(pool);

    while (reader->remaining) {
        size_t len;
        mkw_membuf_t *content;

        MKW_ERR_WRAP(mkw_memreader_eat_cstr(reader, KEY_PREFIX),
                     MKW_ERROR_PAYLOAD_MALFORMED);
        MKW_ERR_WRAP(mkw_memreader_readline(reader, key, "\n"),
                     MKW_ERROR_PAYLOAD_MALFORMED);

        MKW_ERR_WRAP(mkw_memreader_eat_cstr(reader, VAL_PREFIX),
                     MKW_ERROR_PAYLOAD_MALFORMED);
        MKW_ERR(read_length(reader, &len));
        
        if (len > reader->remaining) {
            return MKW_ERROR_PAYLOAD_BAD_LENGTH;
        }

        content = mkw_membuf_create(pool, len);
        MKW_ERR_WRAP(mkw_memreader_read_buf(reader,
                                            mkw_membuf_write_buf(content, len),
                                            len),
                     MKW_ERROR_PAYLOAD_MALFORMED);
        MKW_ERR_WRAP(mkw_memreader_eat_cstr(reader, "\n"),
                     MKW_ERROR_PAYLOAD_MALFORMED);
    }

    return MKW_ERROR_NONE;
}
