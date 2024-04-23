#ifndef GRID_PATTERN_H
#define GRID_PATTERN_H

typedef struct {
	int rows[9];
	int digits[9][9];
} GridPattern;

extern int bestTopRowScore(GridPattern& p) {
	//returns the smallest row after canonicalization of each row independently
	int x;
	int amin = minCanNineBits[p.rows[0]];
	if (amin > (x = minCanNineBits[p.rows[1]])) amin = x;
	if (amin > (x = minCanNineBits[p.rows[2]])) amin = x;
	if (amin > (x = minCanNineBits[p.rows[3]])) amin = x;
	if (amin > (x = minCanNineBits[p.rows[4]])) amin = x;
	if (amin > (x = minCanNineBits[p.rows[5]])) amin = x;
	if (amin > (x = minCanNineBits[p.rows[6]])) amin = x;
	if (amin > (x = minCanNineBits[p.rows[7]])) amin = x;
	if (amin > (x = minCanNineBits[p.rows[8]])) amin = x;
	return amin;
}

extern int fromString(const char* txt, GridPattern& normal, GridPattern& transposed) {
	int src = 0; //pointer to a character in the given text
	int nGivens = 0;
	//gridPattern& normal = pair[0];
	//gridPattern& transposed = pair[1];
	transposed.rows[0] = 0;
	transposed.rows[1] = 0;
	transposed.rows[2] = 0;
	transposed.rows[3] = 0;
	transposed.rows[4] = 0;
	transposed.rows[5] = 0;
	transposed.rows[6] = 0;
	transposed.rows[7] = 0;
	transposed.rows[8] = 0;
	for (int row = 0; row < 9; row++) {
		int r = 0;
		for (int col = 0; col < 9; col++) {
			int c = txt[src]; //read the character src points to
			if (c >= '1' && c <= '9') { //it is a "given", when input is bytes 0..9
				nGivens++;
				r |= (1 << (8 - col)); //most significant bit is for c0, less significant for c8
				transposed.rows[col] |= (1 << (8 - row));
				normal.digits[row][col] = transposed.digits[col][row] = c - '0'; //when input is ASCII
			}
			else {
				normal.digits[row][col] = transposed.digits[col][row] = 0;
			}
			src++; //move to next char
		}
		normal.rows[row] = r;
	}
	return nGivens;
}

#endif // !GRID_PATTERN_H
