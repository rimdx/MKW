#include "mkw.h"
#include "mkw_pgp.h"

static void 
write_new_len(mkw_membuf_t *buf, uint64_t len)
{
    if (len < 192) {
        mkw_membuf_write_uint8(buf, (uint8_t)len);
    } else if (len <= ((223 - 192) << 8) + 0xFF + 192) {
        len -= 192;
        mkw_membuf_write_uint8(buf, (uint8_t)((len >> 8 & 0xff) + 192));
        mkw_membuf_write_uint8(buf, (uint8_t)(len & 0xff));
    } else {
        mkw_membuf_write_uint8(buf, 0xff);
        mkw_membuf_write_uint32(buf, len);
    }
}

static mkw_error_t 
read_new_len(mkw_memreader_t *reader, uint64_t *len)
{
    uint8_t b0, b1;
    MKW_ERR(mkw_memreader_read_uint8(reader, &b0));

    if (b0 < 192) {
        *len = b0;
        return MKW_ERROR_NONE;
    } else if (b0 < 224) {
        MKW_ERR(mkw_memreader_read_uint8(reader, &b1));
        *len = (((b0 - 192) << 8) | b1) + 192;
        return MKW_ERROR_NONE;
    } else if (b0 == 255) {
        uint32_t u32_len;
        MKW_ERR(mkw_memreader_read_uint32(reader, &u32_len));
        *len = u32_len;
        return MKW_ERROR_NONE;
    } else {
        return MKW_ERROR_PARTIAL_LENGTH_NOT_SUPPORTED;
    }
}

void
mkw_pgp_packet_serialize(mkw_membuf_t *buf,
                         enum mkw_pgp_packet_tag_e tag,
                         uint8_t *body, size_t bodylen)
{
    assert(tag <= 0x3f);
    mkw_membuf_write_uint8(buf, 
                           (0x80) | /* always one*/
                           (0x40) | /* new format */
                           (0x3f & tag));

    write_new_len(buf, bodylen);
    mkw_membuf_write_str(buf, body, bodylen);
}

mkw_error_t
mkw_pgp_packet_deserialize(mkw_memreader_t *reader,
                           enum mkw_pgp_packet_tag_e *tag,
                           mkw_memreader_t *body)
{
    uint8_t header;
    size_t len;

    MKW_ERR(mkw_memreader_read_uint8(reader, &header));
    int just_one    = 0x80 & header;
    int new_format  = 0x40 & header;
    *tag            = 0x3f & header;

    if (! (just_one && new_format)) {
        return MKW_ERROR_MAFORMED_PACKET;
    }

    MKW_ERR(read_new_len(reader, &len));
    MKW_ERR(mkw_memreader_subreader(reader, body, len));

    return MKW_ERROR_NONE;
}
