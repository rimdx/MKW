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
#define LIMB_BACKWARD(bigint, index) bigint->digits[bigint->limbs - 1 - (index)]

#undef max
static int
max(int a, int b) {
    return (a > b) ? a : b;
}

mkw_bigint_t *
mkw_bigint_create(int limbs, mkw_pool_t *pool)
{
    mkw_bigint_t *bigint = mkw_pcalloc(pool, sizeof(*bigint));
    bigint->limbs = countof(bigint->digits);
    return bigint;
}

mkw_bigint_t *
mkw_bigint_create_empty(mkw_pool_t *pool)
{
    mkw_bigint_t *bigint = mkw_pcalloc(pool, sizeof(*bigint));
    bigint->limbs = countof(bigint->digits);
    return bigint;
}

mkw_bigint_t *
mkw_bigint_dup(const mkw_bigint_t *n, mkw_pool_t *pool)
{
    mkw_bigint_t *result = mkw_bigint_create(n->limbs, pool);
    mkw_bigint_set(result, n);
    return result;
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
    for (int i = 0; i < size; i++) {
        bigint->digits[size - i - 1] = data[i];
    }
    return bigint;
}

mkw_bigint_t *
mkw_bigint_set(mkw_bigint_t *x, const mkw_bigint_t *n)
{
    memset(x->digits, 0, x->limbs * sizeof(mkw_limb_t));
    memcpy(x->digits + x->limbs - n->limbs, n->digits, n->limbs * sizeof(mkw_limb_t));
    return x;
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
    int srcsize = x->limbs;

    memmove(&x->digits[maybe_move_right],
            &x->digits[maybe_move_left],
            max(x->limbs - abs(n), 0) * sizeof(mkw_limb_t));

    /* strip leftover parts */
    memset(&x->digits[(n > 0) ? x->limbs - abs(n) : 0], 0,
           abs(n) * sizeof(mkw_limb_t));
}

int
mkw_bigint_getbit(const mkw_bigint_t *x, int n)
{
    return x->digits[n / LIMB_BITS] & (1 << (n % LIMB_BITS));
}

void
mkw_bigint_setbit(mkw_bigint_t *x, int n, int v)
{
    int limb = LIMB_BACKWARD(x, n / LIMB_BITS);
    limb = (limb & ~(1 << (n % LIMB_BITS))) | (v << (n % LIMB_BITS));
    x->digits[n / LIMB_BITS] = limb;
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

void
mkw_bigint_print(const mkw_bigint_t *num, FILE *file)
{
    for (int i = countof(num->digits); i >= 0; i--) {
        fprintf(file, "%08x ", num->digits[i]);
    }
    putc('\n', file);
}

void
mkw_bigint_add(mkw_bigint_t *x, const mkw_bigint_t *n)
{
    mkw_biglimb_t carry = 0;
    int i;

    for (i = 0; i < x->limbs; i++) {
        mkw_biglimb_t a = (i < x->limbs) ? x->digits[i] : 0;
        mkw_biglimb_t b = (i < n->limbs) ? n->digits[i] : 0;
        mkw_biglimb_t sum = a + b + carry;
        x->digits[i] = sum & LS_LIMB_MASK;
        carry = (sum & MS_LIMB_MASK) >> LIMB_BITS;
    }
    assert(carry == 0);
}

void
mkw_bigint_add_n(mkw_bigint_t *x, mkw_limb_t n)
{
    mkw_biglimb_t carry = n;
    int i;

    for (i = 0; i < x->limbs && carry; i++) {
        mkw_biglimb_t a = (i < x->limbs) ? x->digits[i] : 0;
        mkw_biglimb_t sum = a + carry;
        x->digits[i] = sum & LS_LIMB_MASK;
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

    for (i = 0; i < x->limbs; i++) {
        mkw_biglimb_t product = x->digits[i];
        product *= n;
        product += carry;
        x->digits[i] = product & LS_LIMB_MASK;
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

    for (i = 0; i < b->limbs / 2; i++) {
        mkw_bigint_set(tmp, a);
        mkw_bigint_mul_n(tmp, b->digits[i]);
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
        mkw_biglimb_t sum = (i < x->limbs) ? x->digits[i] : 0;
        sum |= ((mkw_biglimb_t)1 << LIMB_BITS); /* set maybe-carry bit */
        sum -= carry; /* perform substraction */

        /* recover result from the least significant component, and carry from
         * most significant if used. it will be zero if it was actually used
         * and one if it persists from the begining of the operation. shift
         * moves it back to the least significant position and XOR inverts
         * this, and only this bit */
        x->digits[i] = sum & LS_LIMB_MASK;
        carry = 1 ^ ((sum & MS_LIMB_MASK) >> LIMB_BITS);
    }

    assert(carry == 0);
}

void
mkw_bigint_sub(mkw_bigint_t *x, const mkw_bigint_t *n)
{
    mkw_biglimb_t borrow = 0;
    int i;

    MKW_BIGINT_TRACE(x);
    MKW_BIGINT_TRACE(n);

    for (i = 0; i < x->limbs; i++) {
        mkw_biglimb_t sum = (1L << LIMB_BITS) | x->digits[i];
        mkw_biglimb_t subtrahend = (i < n->limbs) ? n->digits[i] : 0;
        sum = sum - borrow - subtrahend;
        x->digits[i] = sum & LS_LIMB_MASK;
        borrow = 1 ^ ((sum & MS_LIMB_MASK) >> LIMB_BITS);
    }

    MKW_BIGINT_TRACE(x);
    assert(borrow == 0); /* prevents the number from being negative */
}

int
mkw_bigint_cmp(const mkw_bigint_t *a, const mkw_bigint_t *b)
{
    /* with the same bitsize, compare actual limbs. please note, that even
     * though bitsize are the same, the amount of limbs might still differ.
     * for examle, the most significant limb might be zeroed and reserved
     * for potential use in future. this still means that (0x00,0x01) and
     * (0x01) are the same bigints (assume 8 bit limbs for convenience). */

    for (int size = 16; size; size--) {
        mkw_limb_t la = a->digits[size];
        mkw_limb_t lb = b->digits[size];
        if (la != lb) {
            return la > lb ? 1 : -1;
        }
    }

    return 0;
}
