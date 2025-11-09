# export GNUPGHOME=$0

dir="$(dirname $0)"

gpg --export --armor 950B77FBB980E4A1 > "$dir/public_key_plaintext.asc"
gpg --export-secret-keys --armor 950B77FBB980E4A1 > "$dir/secret_key_plaintext.asc"

gpg --encrypt --armor --recipient 950B77FBB980E4A1 \
    --output "$dir/message_pubkey_aes128_nocompression.asc" \
    --yes \
    --cipher-algo AES-128 \
    --compress-algo none "$dir/message_paintext.txt"

gpg --encrypt --armor --recipient 950B77FBB980E4A1 \
    --output "$dir/message_pubkey_aes256_nocompression.asc" \
    --yes \
    --cipher-algo AES-256 \
    --compress-algo none "$dir/message_paintext.txt"

gpg --batch --symmetric --passphrase "123" \
    --output "$dir/message_symkey_aes128_nocompression.asc" \
    --yes \
    --cipher-algo AES-128 \
    --compress-algo none "$dir/message_paintext.txt"

gpg --batch --symmetric --passphrase "123" \
    --output "$dir/message_symkey_aes256_nocompression.asc" \
    --yes \
    --cipher-algo AES-256 \
    --compress-algo none "$dir/message_paintext.txt"
