#include "mkw.h"
#include "mkw_crypto.h"

typedef uint64_t mkw_biglimb_t;
#define LS_LIMB_MASK 0x00000000ffffffff
#define MS_LIMB_MASK 0xffffffff00000000

#undef max
static int
max(int a, int b) {
    return (a > b) ? a : b;
}

mkw_bigint_t *
mkw_bigint_set(mkw_bigint_t *x, const mkw_bigint_t *n)
{
    memcpy(x->digits, n->digits, sizeof(x->digits));
    return x;
}

static int
decrypt_one_hex(char ch)
{
    switch (ch) {
        case '0': return 0;
        case '1': return 1;
        case '2': return 2;
        case '3': return 3;
        case '4': return 4;
        case '5': return 5;
        case '6': return 6;
        case '7': return 7;
        case '8': return 8;
        case '9': return 9;
        case 'a': return 10;
        case 'b': return 11;
        case 'c': return 12;
        case 'd': return 13;
        case 'e': return 14;
        case 'f': return 15;
        default: abort();
    };
}

mkw_bigint_t *
mkw_bigint_set_hex(mkw_bigint_t *x, const char *data)
{
    int i;
    int len = strlen(data);
    int limb_nibbles = sizeof(mkw_limb_t) * 2;
    assert(len % 2 == 0);
    assert(len / 2 < sizeof(*x));

    memset(x->digits, 0, sizeof(x->digits));
    for (i = 0; i < len; i++) {
        int num = decrypt_one_hex(data[i]);
        x->digits[i / limb_nibbles] |= num << (i % limb_nibbles * 4);
    }
    return x;
}

void
mkw_bigint_swap(mkw_bigint_t *a, mkw_bigint_t *b)
{
    for (int i = 0; i < MKW_BIGINT_LIMBS; i++) {
        mkw_limb_t tmp = a->digits[i];
        a->digits[i] = b->digits[i];
        b->digits[i] = tmp;
    }
}

void
mkw_bigint_limbshift(mkw_bigint_t *x, int n)
{
    int maybe_move_right = (n > 0) ? abs(n) : 0;
    int maybe_move_left = (n < 0) ? abs(n) : 0;

    memmove(&x->digits[maybe_move_right],
            &x->digits[maybe_move_left],
            max(MKW_BIGINT_LIMBS - abs(n), 0) * sizeof(mkw_limb_t));

    /* strip leftover parts */
    memset(&x->digits[(n < 0) ? MKW_BIGINT_LIMBS - abs(n) : 0], 0,
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
mkw_bigint_bitsize(const mkw_bigint_t *x)
{
    int i = MKW_BIGINT_LIMBS - 1;
    for (int i = 0; i >= 0; i--) {
        if (x->digits[i]) {
            return i + mkw_limb_bitsize(x->digits[i]);
        }
    }
    return 0;
}

int
mkw_bigint_getbit(const mkw_bigint_t *x, int n)
{
    mkw_limb_t limb = x->digits[n / MKW_BIGINT_LIMB_BITS];
    int mask = 1 << (n % MKW_BIGINT_LIMB_BITS);
    return (limb & mask) ? 1 : 0;
}

void
mkw_bigint_print(const mkw_bigint_t *num, FILE *file)
{
    for (int i = 6; i >= 0; i--) {
        fprintf(file, "%08x ", num->digits[i]);
    }
    putc('\n', file);
}

void
mkw_bigint_add(mkw_bigint_t *x, const mkw_bigint_t *n)
{
    mkw_biglimb_t carry = 0;
    int i;

    for (i = 0; i < MKW_BIGINT_LIMBS; i++) {
        mkw_biglimb_t a = (i < MKW_BIGINT_LIMBS) ? x->digits[i] : 0;
        mkw_biglimb_t b = (i < MKW_BIGINT_LIMBS) ? n->digits[i] : 0;
        mkw_biglimb_t sum = a + b + carry;
        x->digits[i] = sum & LS_LIMB_MASK;
        carry = (sum & MS_LIMB_MASK) >> MKW_BIGINT_LIMB_BITS;
    }
    assert(carry == 0);
}

void
mkw_bigint_add_n(mkw_bigint_t *x, mkw_limb_t n)
{
    mkw_biglimb_t carry = n;
    int i;

    for (i = 0; i < MKW_BIGINT_LIMBS && carry; i++) {
        mkw_biglimb_t sum = x->digits[i] + carry;
        x->digits[i] = sum & LS_LIMB_MASK;
        carry = (sum & MS_LIMB_MASK) >> MKW_BIGINT_LIMB_BITS;
    }
}

void mkw_bigint_modadd(mkw_bigint_t *x,
                       const mkw_bigint_t *a,
                       const mkw_bigint_t *b,
                       const mkw_bigint_t *p)
{
    mkw_bigint_add(mkw_bigint_set(x, a), b);
    if (mkw_bigint_cmp(x, p) > 0) {
        mkw_bigint_sub(x, p);
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

    for (i = 0; i < MKW_BIGINT_LIMBS; i++) {
        mkw_biglimb_t product = x->digits[i];
        product *= n;
        product += carry;
        x->digits[i] = product & LS_LIMB_MASK;
        carry = (product & MS_LIMB_MASK) >> MKW_BIGINT_LIMB_BITS;
    }
    assert(carry == 0);
}

void
mkw_bigint_mul(mkw_bigint_t *x, const mkw_bigint_t *a,
               const mkw_bigint_t *b)
{
    int i;
    memset(x->digits, 0, sizeof(x->digits));

    for (i = 0; i < MKW_BIGINT_LIMBS / 2; i++) {
        mkw_bigint_t tmp;
        mkw_bigint_set(&tmp, a);
        mkw_bigint_mul_n(&tmp, b->digits[i]);
        mkw_bigint_limbshift(&tmp, i);
        mkw_bigint_add(x, &tmp);
    }
}

void
mkw_bigint_sub_n(mkw_bigint_t *x, mkw_limb_t n)
{
    mkw_biglimb_t carry = n;
    int i;

    for (i = 0; i < MKW_BIGINT_LIMBS && carry; i++) {
        mkw_biglimb_t sum = x->digits[i];
        sum |= ((mkw_biglimb_t)1 << MKW_BIGINT_LIMB_BITS); /* set maybe-carry bit */
        sum -= carry; /* perform substraction */

        /* recover result from the least significant component, and carry from
         * most significant if used. it will be zero if it was actually used
         * and one if it persists from the begining of the operation. shift
         * moves it back to the least significant position and XOR inverts
         * this, and only this bit */
        x->digits[i] = sum & LS_LIMB_MASK;
        carry = 1 ^ ((sum & MS_LIMB_MASK) >> MKW_BIGINT_LIMB_BITS);
    }

    assert(carry == 0);
}

void
mkw_bigint_sub(mkw_bigint_t *x, const mkw_bigint_t *n)
{
    mkw_biglimb_t borrow = 0;
    int i;

    for (i = 0; i < MKW_BIGINT_LIMBS; i++) {
        mkw_biglimb_t sum = (1L << MKW_BIGINT_LIMB_BITS) | x->digits[i];
        sum = sum - borrow - n->digits[i];
        x->digits[i] = sum & LS_LIMB_MASK;
        borrow = 1 ^ ((sum & MS_LIMB_MASK) >> MKW_BIGINT_LIMB_BITS);
    }

    assert(borrow == 0); /* prevents the number from being negative */
}

void mkw_bigint_modsub(mkw_bigint_t *x,
                       const mkw_bigint_t *a,
                       const mkw_bigint_t *b,
                       const mkw_bigint_t *p)
{
    if (mkw_bigint_cmp(a, b) > 0) {
        mkw_bigint_sub(mkw_bigint_set(x, a), b);
    } else {
        mkw_bigint_sub(mkw_bigint_set(x, b), a);
    }
}

int
mkw_bigint_cmp(const mkw_bigint_t *a, const mkw_bigint_t *b)
{
    int size, result = 0;

    /* with the same bitsize, compare actual limbs. please note, that even
     * though bitsize are the same, the amount of limbs might still differ.
     * for examle, the most significant limb might be zeroed and reserved
     * for potential use in future. this still means that (0x00,0x01) and
     * (0x01) are the same bigints (assume 8 bit limbs for convenience). */

    for (size = 15; size >= 0; size--) {
        mkw_limb_t la = a->digits[size];
        mkw_limb_t lb = b->digits[size];
        if (la != lb) {
            result = la > lb ? 1 : -1;
            break;
        }
    }

    return result;
}

#define BIGLIMB(low, high) (((mkw_biglimb_t)low << MKW_BIGINT_LIMB_BITS) | (high))

void
mkw_bigint_div(mkw_bigint_t *result, mkw_bigint_t *remainder,
               const mkw_bigint_t *x, const mkw_bigint_t *n)
{
    /* nsize is more like an index actually. it holds position of first
     * non-zero limb index of what we divide by. this is imprortant for the
     * whole thing to function because we want to avoid dividing by zero. and
     * this is just how the algorithm generally works. */
    int nsize = MKW_BIGINT_LIMBS - 1;
    int i;
    mkw_biglimb_t nword;

    memset(remainder->digits, 0, sizeof(remainder->digits));
    memset(result->digits, 0, sizeof(result->digits));

    /* skips all zeres from n to measure its real size. also capture nword */
    while ((nword = n->digits[nsize]) == 0) {
        nsize--;

        /* this basically signifies that the number we divide by is zero which
         * is illegal and banned by convention of our implementation. we're not
         * going to make any speacial cases to handle that nor do we need any
         * in the real world. */
        assert(nsize >= 0);
    }

    /* division is rare case when the operaion actually start from the lowest
     * limb. substract one cuz it's an index. */
    for (i = countof(x->digits) - 1; i >= 0; i--) {
        mkw_biglimb_t xdword, quotient;
        mkw_bigint_t product = { 0 };

        /* leftshift and move new number into the newly emptied slot */
        memmove(&remainder->digits[1],
                &remainder->digits[0],
                countof(remainder->digits) - 1);
        remainder->digits[0] = x->digits[i];

        /* that's weird to add one to the first component (lower) when parsing
         * a number but that is the life with little endian. the xdword number
         * must capture one more digit then the nword */
        xdword = BIGLIMB(remainder->digits[nsize + 1],
                         remainder->digits[nsize]);

        /* cut of if result tends to be more than it fits into a single limb.
         * this is possible but out of range number will never be the answer to
         * what we want to write into the digit. */
        quotient = min(xdword / nword, LS_LIMB_MASK);

        /* here we recover the digits with a hint that we currently have in the
         * quotient and the formula (x=n*q+r). we just iterate until the
         * conditions meet. (product that we guessed should become less than
         * the other factor (remainder in our case) -> while[p > n]). the
         * remainder and the quotient (answer to the short the guessed number)
         * are adjusted in process. */

        /* product = n * quotient */
        mkw_bigint_mul_n(mkw_bigint_set(&product, n), quotient);
        /* while [product > remainder] */
        while (mkw_bigint_cmp(&product, remainder) > 0) {
            mkw_bigint_sub(&product, n);
            quotient--;
        }

        mkw_bigint_sub(remainder, &product);
        result->digits[i] = quotient;
    }
}

void mkw_bigint_modmul(mkw_bigint_t *x, const mkw_bigint_t *a,
                       const mkw_bigint_t *b, const mkw_bigint_t *p)
{
    mkw_bigint_t tmp, discard;
    mkw_bigint_mul(&tmp, a, b);
    mkw_bigint_div(x, &discard, &tmp, p);
}

/* Modular inverse using the Extended Euclidean Algorithm [1].
 *
 * This is basically a way to divide numbers in modular arithmentic. In fancier
 * way we can say that it finds some t that satisfies this equation:
 *
 * a*t=1 mod n
 *
 * [1] https://en.wikipedia.org/wiki/Extended_Euclidean_algorithm#Modular_integers */
void mkw_bigint_inv(mkw_bigint_t *x,
                    const mkw_bigint_t *a,
                    const mkw_bigint_t *n)
{
    mkw_bigint_t t = { 0 };
    mkw_bigint_t r = { 0 };
    mkw_bigint_t newt = { 0 };
    mkw_bigint_t newr = { 0 };
    int sign = 1;

    mkw_bigint_set(&r, n);
    newt.digits[0] = 1;
    mkw_bigint_set(&newr, a);

    while (mkw_bigint_bitsize(&newr) != 0) {
        mkw_bigint_t quotient, discard_remainder, tmp;

        MKW_BIGINT_TRACE(&t);
        MKW_BIGINT_TRACE(&r);

        /* quotient := r div newr */
        mkw_bigint_div(&quotient, &discard_remainder, &r, &newr);

        /* 
         * (t, newt) := (newt, t − quotient × newt) 
         * (r, newr) := (newr, r − quotient × newr)
         *
         * tmp = quotient * new##(t|r)##
         */
        mkw_bigint_mul(&tmp, &quotient, &newt);
        mkw_bigint_add(&t, &tmp);
        mkw_bigint_swap(&t, &newt);

        mkw_bigint_mul(&tmp, &quotient, &newr);
        mkw_bigint_sub(&r, &tmp);
        mkw_bigint_swap(&r, &newr);

        sign = -sign;
    }

    if (sign > 0) {
        mkw_bigint_set(x, n);
        mkw_bigint_sub(x, &t);
    } else {
        mkw_bigint_set(x, &t);
    }
}
