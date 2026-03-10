#include "mkw.h"
#include "mkw_crypto.h"

typedef uint64_t mkw_biglimb_t;
#define LS_LIMB_MASK 0x00000000ffffffff
#define MS_LIMB_MASK 0xffffffff00000000
#define bitsize(type) (sizeof(type) * CHAR_BIT)
#define LIMBS_FROM_BITSIZE(bits) \
    ((bits + bitsize(mkw_limb_t) - 1) / bitsize(mkw_limb_t))
#define LIMB_BITS bitsize(mkw_limb_t)

#define LIMB_FORWARD(bigint, index) bigint->digits[index]
#define LIMB_BACKWARD(bigint, index) bigint->digits[bigint->limbs - 1 - index]

#undef max
static int
max(int a, int b) {
    return (a > b) ? a : b;
}

mkw_bigint_t *
mkw_bigint_create(int limbs, mkw_pool_t *pool)
{
    mkw_bigint_t *bigint = mkw_pcalloc(pool, sizeof(*bigint));
    bigint->limbs = limbs;
    bigint->digits = mkw_pcalloc(pool, bigint->limbs * sizeof(mkw_limb_t));
    bigint->pool = pool;
    return bigint;
}

mkw_bigint_t *
mkw_bigint_dup(const mkw_bigint_t *n, mkw_pool_t *pool)
{
    mkw_bigint_t *result = mkw_bigint_create(n->limbs, pool);
    mkw_bigint_set(result, n);
    return result;
}

void
mkw_bigint_reserve_limbs(mkw_bigint_t *num, int limbs)
{
    if (num->limbs < limbs) {
        int offset = limbs - num->limbs;
        mkw_limb_t *buf = mkw_pcalloc(num->pool, limbs * sizeof(mkw_limb_t));
        memcpy(buf + offset, num->digits, num->limbs * sizeof(mkw_limb_t));
        num->digits = buf;
        num->limbs = limbs;
    }
}

void
mkw_bigint_reserve_bits(mkw_bigint_t *num, int bits)
{
    mkw_bigint_reserve_limbs(num, LIMBS_FROM_BITSIZE(bits));
}

mkw_bigint_t *
mkw_bigint_from_num(mkw_limb_t num, mkw_pool_t *pool)
{
    mkw_bigint_t *bigint = mkw_bigint_create(1, pool);
    bigint->digits[0] = num;
    return bigint;
}

mkw_bigint_t *
mkw_bigint_from_limbs(const mkw_limb_t *data, mkw_limb_t size,
                      mkw_pool_t *pool)
{
    mkw_bigint_t *bigint = mkw_bigint_create(size, pool);
    memcpy(bigint->digits, data, size * sizeof(mkw_limb_t));
    return bigint;
}

void
mkw_bigint_set(mkw_bigint_t *x, const mkw_bigint_t *n)
{
    mkw_bigint_reserve_limbs(x, n->limbs);
    memset(x->digits, 0, x->limbs * sizeof(mkw_limb_t));
    memcpy(x->digits + x->limbs - n->limbs, n->digits, n->limbs * sizeof(mkw_limb_t));
}

void
mkw_bigint_zero(mkw_bigint_t *x)
{
    memset(x->digits, 0, x->limbs * sizeof(mkw_limb_t));
}

void
mkw_bigint_limbshift(mkw_bigint_t *x, int n)
{
    int maybe_move_right = (n < 0) ? abs(n) : 0;
    int maybe_move_left = (n > 0) ? abs(n) : 0;
    int bitsize = mkw_bigint_bitsize(x);
    int srcsize = x->limbs;

    mkw_bigint_reserve_limbs(x, LIMBS_FROM_BITSIZE(bitsize) + maybe_move_left);

    memmove(&x->digits[maybe_move_right],
            &x->digits[maybe_move_left],
            (x->limbs - abs(n)) * sizeof(mkw_limb_t));

    /* strip leftover parts */
    memset(&x->digits[(n > 0) ? x->limbs - abs(n) : 0], 0,
           abs(n) * sizeof(mkw_limb_t));
}

int
mkw_limb_bitsize(mkw_limb_t limb)
{
    int size = bitsize(mkw_limb_t);
    int mask = 1 << (size - 1);
    while (size && ! (limb & mask)) { 
        /* safe way to write right shift with care to overflows. compilers
         * should optimise it out anyway. */
        mask /= 2;
        size--;
    }
    return size;
}

int
mkw_bigint_bitsize(const mkw_bigint_t *num)
{
    int size = num->limbs * bitsize(mkw_limb_t);
    mkw_limb_t *limb = num->digits;
    while (size) {
        size -= bitsize(mkw_limb_t);
        if (*limb) {
            return size + mkw_limb_bitsize(*limb);
        }
        limb++;
    }
    return 0;
}

void
mkw_bigint_print(const mkw_bigint_t *num, FILE *file)
{
    int i;
    for (i = 0; i < 9 * (8 - num->limbs); i++) {
        putc(' ', file);
    }
    for (i = 0; i < num->limbs; i++) {
        // if (i > 0 && i % 4 == 0) {
        //     putc('\n', file);
        // }
        fprintf(file, "%08x ", num->digits[i]);
    }
    putc('\n', file);
}

void
mkw_bigint_add(mkw_bigint_t *x, const mkw_bigint_t *n)
{
    mkw_biglimb_t carry = 0;
    int i;
    int size = max(mkw_bigint_bitsize(x), mkw_bigint_bitsize(n));
    mkw_bigint_reserve_bits(x, size + 1);

    for (i = 0; i < x->limbs; i++) {
        mkw_biglimb_t a = (i < x->limbs) ? LIMB_BACKWARD(x, i) : 0;
        mkw_biglimb_t b = (i < n->limbs) ? LIMB_BACKWARD(n, i) : 0;
        mkw_biglimb_t sum = a + b + carry;
        LIMB_BACKWARD(x, i) = sum & LS_LIMB_MASK;
        carry = (sum & MS_LIMB_MASK) >> LIMB_BITS;
    }
    assert(carry == 0);
}

void
mkw_bigint_add_n(mkw_bigint_t *x, mkw_limb_t n)
{
    mkw_biglimb_t carry = n;
    int i;
    int size = max(mkw_bigint_bitsize(x), mkw_limb_bitsize(n));
    mkw_bigint_reserve_bits(x, size + 1);

    for (i = 0; i < x->limbs && carry; i++) {
        mkw_biglimb_t a = (i < x->limbs) ? LIMB_BACKWARD(x, i) : 0;
        mkw_biglimb_t sum = a + carry;
        LIMB_BACKWARD(x, i) = sum & LS_LIMB_MASK;
        carry = (sum & MS_LIMB_MASK) >> LIMB_BITS;
    }
}

void
mkw_bigint_mul_n(mkw_bigint_t *x, mkw_limb_t n)
{
    mkw_biglimb_t carry = 0;
    int i;

    /* this is replication of long multiplication algorithm. basically the idea
     * is to traverse all numbers in reverse order, starting from the least
     * significant and multiply each of them by a factor, assuming one-digit
     * factor. if this results an overflow, produce a carry digit. it is
     * strictly one word/digit wide. for example in decimal 9*9=81 (c=8)
     * 9*9+8=90 (c=9) this makes 99*9=891.
     *
     * the same thing in hex. consider this example: 0xff_ff*2=0x01_ff_fe.
     * 0xff*2=0x01fe, (r=fe, c=01).
     *
     * sizewise the worst case scenario would be the sum of sizes of two
     * numbers (works for 0*0 as well).
     * */

    mkw_bigint_reserve_bits(x, mkw_bigint_bitsize(x) + mkw_limb_bitsize(n));
    for (i = 0; i < x->limbs; i++) {
        mkw_biglimb_t product = LIMB_BACKWARD(x, i);
        product *= n;
        product += carry;
        LIMB_BACKWARD(x, i) = product & LS_LIMB_MASK;
        carry = (product & MS_LIMB_MASK) >> LIMB_BITS;
    }
    assert(carry == 0);
}

void
mkw_bigint_mul(mkw_bigint_t *x, const mkw_bigint_t *a,
               const mkw_bigint_t *b, mkw_bigint_t *tmp)
{
    int i;
    mkw_bigint_zero(x);

    for (i = 0; i < b->limbs; i++) {
        mkw_bigint_set(tmp, a);
        mkw_bigint_mul_n(tmp, LIMB_BACKWARD(b, i));
        mkw_bigint_limbshift(tmp, i);
        mkw_bigint_add(x, tmp);
    }
}

void
mkw_bigint_sub_n(mkw_bigint_t *x, mkw_limb_t n)
{
    mkw_biglimb_t carry = n;
    int i;

    for (i = 0; i < x->limbs && carry; i++) {
        mkw_biglimb_t sum = (i < x->limbs) ? LIMB_BACKWARD(x, i) : 0;
        sum |= ((mkw_biglimb_t)1 << LIMB_BITS); /* set maybe-carry bit */
        sum -= carry; /* perform substraction */

        /* recover result from the least significant component, and carry from
         * most significant if used. it will be zero if it was actually used
         * and one if it persists from the begining of the operation. shift
         * moves it back to the least significant position and XOR inverts
         * this, and only this bit */
        LIMB_BACKWARD(x, i) = sum & LS_LIMB_MASK;
        carry = 1 ^ ((sum & MS_LIMB_MASK) >> LIMB_BITS);
    }
}

int
mkw_bigint_cmp(const mkw_bigint_t *a, const mkw_bigint_t *b)
{
    int sa = mkw_bigint_bitsize(a);
    int sb = mkw_bigint_bitsize(a);
    if (sa != sb) {
        /* larger bitsizes signifie larger integer value */
        return sa - sb;
    } else {
        /* with the same bitsize, redirect logic to memcmp. please note, that
         * even though bitsize are the same, the amount of limbs might still
         * differ. for examle, the most significant limb might be zeroed and
         * reserved for potential use in future. this still means that
         * (0x00,0x01) and (0x01) are the same bigints (assume 8 bit limbs for
         * convenience). */
        int size = min(a->limbs, b->limbs);
        return memcmp(a->digits + (a->limbs - size),
                      b->digits + (b->limbs - size),
                      size * sizeof(mkw_limb_t));
    }
}
