#ifndef MIN_LEX_CANDIDATE_H
#define MIN_LEX_CANDIDATE_H

#include "bestTripletPermutation.h"
#include "constants.h"
#include "gridPattern.h"

typedef struct {
	char isTransposed;		//0=original source, 1=transposed source
	char mapRowsForward[9]; //source row N goes to target row mapRowsForward[N]
	char mapRowsBackward[9]; //target row M comes from source row mapRowsBackward[M]
	char stacksPerm;
	unsigned char colsPermMask[3];	//bitmask of size 6 with still allowed permutations that don't affect the upper part of the result
} MinLexCandidate;

extern void initCandidate(MinLexCandidate* pThis, int transpose, int topRow) {
	MinLexCandidate defaultInstance = { 0, {-1,-1,-1,-1,-1,-1,-1,-1,-1}, {-1,-1,-1,-1,-1,-1,-1,-1,-1}, 63, {63,63,63} };
	*pThis = defaultInstance;
	pThis->isTransposed = transpose;
	pThis->mapRowsForward[topRow] = 0;
	pThis->mapRowsBackward[0] = topRow;
}

extern void expandStacks(MinLexCandidate* pThis, const GridPattern* const pair, int topKey, MinLexCandidate* results, int& nResults) {
	//for a top row, obtain stack and cols permutations
	const GridPattern& gr = pair[(int)pThis->isTransposed];
	int rowGivens = gr.rows[(int)pThis->mapRowsBackward[0]];
	for (int stackPerm = 0; stackPerm < 6; stackPerm++) {
		int toTriplets[3] = { 0 };
		toTriplets[perm[stackPerm][0]] = (rowGivens >> 6) & 7;
		toTriplets[perm[stackPerm][1]] = (rowGivens >> 3) & 7;
		toTriplets[perm[stackPerm][2]] = (rowGivens >> 0) & 7;
		const BestTripletPermutation& bt0 = bestTripletPermutations[toTriplets[0]][63];
		if (bt0.bestResult > ((topKey >> 6) & 7))
			continue;
		const BestTripletPermutation& bt1 = bestTripletPermutations[toTriplets[1]][63];
		if (bt1.bestResult > ((topKey >> 3) & 7))
			continue;
		const BestTripletPermutation& bt2 = bestTripletPermutations[toTriplets[2]][63];
		if (bt2.bestResult > ((topKey >> 0) & 7))
			continue;
		//this stack permutation results in minimal top row. Store the expanded candidate.
		MinLexCandidate& res = results[nResults++];
		res = *pThis;
		res.stacksPerm = stackPerm;
		res.colsPermMask[0] = bt0.resultMask;
		res.colsPermMask[1] = bt1.resultMask;
		res.colsPermMask[2] = bt2.resultMask;
	}
}
#endif // !MIN_LEX_CANDIDATE_H