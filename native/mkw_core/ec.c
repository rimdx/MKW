#include "mkw.h"
#include "mkw_crypto.h"

void
mkw_ecc_curve_nistp256(mkw_ecc_curve_t *x)
{
    /* https://std.neuromancer.sk/nist/P-256 */
    mkw_bigint_set_hex(x->p,
        "ffffffff00000001000000000000000000000000ffffffffffffffffffffffff");
    mkw_bigint_set_hex(x->a,
        "ffffffff00000001000000000000000000000000fffffffffffffffffffffffc");
    mkw_bigint_set_hex(x->a,
        "5ac635d8aa3a93e7b3ebbd55769886bc651d06b0cc53b0f63bce3c3e27d2604b");
    mkw_bigint_set_hex(x->g->x,
        "6b17d1f2e12c4247f8bce6e563a440f277037d812deb33a0f4a13945d898c296");
    mkw_bigint_set_hex(x->g->y,
        "4fe342e2fe1a7f9b8ee7eb4a7c0f9e162bce33576b315ececbb6406837bf51f5");
}

void mkw_ecc_point_add(mkw_ecc_point_t *result,
                       const mkw_ecc_curve_t *curve,
                       const mkw_ecc_point_t *p,
                       const mkw_ecc_point_t *q)
{
    mkw_bigint_t slope, slope_squared, tmp;

    if (p->is_infinity || q->is_infinity) {
        /* if any point of two points we add is at infinity, the result would
         * always be at infinity as well no matter what. hence, easy out. */
        result->is_infinity = 1;
        return;
    } else if (mkw_bigint_cmp(p->x, q->x) == 1 || mkw_bigint_cmp(p->y, q->y)) {
        /* slope = (3*[p.x]^2 + a) / 2*[p.y] */

    } else {
        /* slope = dy/dx = (y2 - y1)/(x2 - x1) */
        mkw_bigint_t dy, dx, dx_inv, discard;

        mkw_bigint_modsub(&dy, p->y, q->y, curve->p);
        mkw_bigint_modsub(&dx, p->x, q->x, curve->p);

        mkw_bigint_inv(&dx_inv, &dx, curve->p);
        mkw_bigint_modmul(&slope, &dy, &dx_inv, curve->p);
    }

    /* r.x = slope^2 - p.x - q.x */
    mkw_bigint_modmul(result->x, &slope, &slope, curve->p);
    mkw_bigint_modsub(result->x,
                      mkw_bigint_set(&tmp, result->x),
                      p->x, curve->p);
    mkw_bigint_modsub(result->x,
                      mkw_bigint_set(&tmp, result->x),
                      q->x, curve->p);

    /* r.y = slope(p.x - r.x) - p.y */
    mkw_bigint_modsub(&tmp, p->x, result->x, curve->p);
    mkw_bigint_modmul(result->y, &tmp, &slope, curve->p);
    mkw_bigint_modsub(result->y, mkw_bigint_set(&tmp, result->y),
                      p->y, curve->p);
}

void mkw_ecc_point_mul(const mkw_ecc_curve_t *curve,
                       mkw_ecc_point_t *result,
                       const mkw_ecc_point_t *pt,
                       const mkw_bigint_t *n)
{
    mkw_bigint_t x, y;
    mkw_ecc_point_t base2 = { 
        .x = mkw_bigint_set(&x, curve->g->x),
        .y = mkw_bigint_set(&y, curve->g->y),
    };

    for (int bit = 0; bit < MKW_BIGINT_BITS; bit++) {
        mkw_bigint_t tmp_x, tmp_y;

        if (mkw_bigint_getbit(n, bit)) {
            /* result += base2 */
            mkw_ecc_point_t tmp_pt = {
                .x = mkw_bigint_set(&tmp_x, result->x),
                .y = mkw_bigint_set(&tmp_y, result->y),
            };
            mkw_ecc_point_add(result, curve, &tmp_pt, &base2);
        }

        {
            /* base2 *= 2 */
            mkw_ecc_point_t tmp_pt = {
                .x = mkw_bigint_set(&tmp_x, base2.x),
                .y = mkw_bigint_set(&tmp_y, base2.y),
            };
            mkw_ecc_point_add(&base2, curve, &tmp_pt, &tmp_pt);
        }
    }
}
