#ifndef BEST_TRIPLET_PERMUTATION_H
#define BEST_TRIPLET_PERMUTATION_H

typedef struct {
	int bestResult; //three bit best allowed composition for the givens within a triplet
	int resultMask; //6-bits mask of permutation indexes with 1 for the allowed permutations and zero otherwise
	int resultNumBits; //number of the possible permutations in the result mask
} BestTripletPermutation;

#endif // !BEST_TRIPLET_PERMUTATION_H