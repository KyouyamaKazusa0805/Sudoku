#include "mapper.h"
#include "minLexCandidate.h"
#include <stdio.h>
#include <stdlib.h>
#include <vector>

//CAND_LIST_SIZE worst case is 15552 = 2 (transpose) * 6 (band permutations) * 6*6*6 (rows in a band perm) * 6 (stack perm)
constexpr auto CAND_LIST_SIZE = 15552;

void patminlex(const char*, char*);
void map(const char*, char*);

std::vector<Mapper> theMaps{};

int main(void) {
	const char* const source = "000040602004800091020000040901008006050000080200700405010000050460003900503020000";
	char result[82] = { '\0' };
	patminlex(source, result);
	puts(result);
	//system("pause");
	return 0;
}

#pragma warning (disable: 6262)
static void patminlex(const char* source, char* result) {
	MinLexCandidate candidates[CAND_LIST_SIZE]; //rows 0,2,4,6,8
	MinLexCandidate candidates1[CAND_LIST_SIZE]; //rows 1,3,5,7
	int minTopRowScores[2], minTopRowScore;
	GridPattern pair[2]; //original and transposed patterns

	int nGivens = fromString(source, pair[0], pair[1]); //compose the pair of the patterns for the original and transposed morph

	theMaps.clear();

	minTopRowScores[0] = bestTopRowScore(pair[0]);
	minTopRowScores[1] = bestTopRowScore(pair[1]);
	if (minTopRowScores[0] <= minTopRowScores[1])
		minTopRowScore = minTopRowScores[0];
	else
		minTopRowScore = minTopRowScores[1];

	result[0] = ((minTopRowScore >> 8) & 1);
	result[1] = ((minTopRowScore >> 7) & 1);
	result[2] = ((minTopRowScore >> 6) & 1);
	result[3] = ((minTopRowScore >> 5) & 1);
	result[4] = ((minTopRowScore >> 4) & 1);
	result[5] = ((minTopRowScore >> 3) & 1);
	result[6] = ((minTopRowScore >> 2) & 1);
	result[7] = ((minTopRowScore >> 1) & 1);
	result[8] = ((minTopRowScore) & 1);

	//step 1: determine top rows
	int nCurCandidates;
	int nNextCandidates;
	nCurCandidates = 0;
	for (int nowTransposed = 0; nowTransposed < 2; nowTransposed++) {
		if (minTopRowScores[nowTransposed] > minTopRowScore)
			continue;
		for (int topRow = 0; topRow < 9; topRow++) {
			if (minCanNineBits[pair[nowTransposed].rows[topRow]] > minTopRowScore)
				continue;
			//here we have a top row candidate
			MinLexCandidate cand;
			init_candidate(&cand, nowTransposed, topRow); //in the empty template fix only the transposition and row 0
			expandStacks(&cand, pair, minTopRowScore, candidates, nCurCandidates); //fix all minimal stack permutations and store for later row expansion
		}
	}
	//here we have all top row candidates with fixed top row, stacks, and column permutation masks

	//step 2: expand rows
	MinLexCandidate* curCandidates = candidates;	//read from here
	MinLexCandidate* nextCandidates = candidates1; //add row and write here
	nNextCandidates = 0;
	for (int toRow = 1; toRow < 9; toRow++) {
		int bestTriplets0 = 7;
		int bestTriplets1 = 7;
		int bestTriplets2 = 7;
		int rowInBand = toRow % 3;
		for (int curCandidateIndex = 0; curCandidateIndex < nCurCandidates; curCandidateIndex++) {
			const MinLexCandidate& old = curCandidates[curCandidateIndex];
			int startRow, endRow;
			if (rowInBand) {
				//combine with unmapped rows from the same band
				int band = old.mapRowsBackward[3 * (toRow / 3)] / 3;
				startRow = band * 3;
				endRow = startRow + 3;
			}
			else {
				//try any unmapped row
				startRow = 0;
				endRow = 9;
			}
			for (int fromRow = startRow; fromRow < endRow; fromRow++) {
				if (old.mapRowsForward[fromRow] >= 0)
					continue; //skip previously mapped rows
				int toTriplets[3] = { 0 };
				int rowGivens = pair[(int)old.isTransposed].rows[fromRow]; //stacks unmapped
				toTriplets[perm[(int)old.stacksPerm][0]] = (rowGivens >> 6);// & 7;
				toTriplets[perm[(int)old.stacksPerm][1]] = (rowGivens >> 3) & 7;
				toTriplets[perm[(int)old.stacksPerm][2]] = (rowGivens >> 0) & 7;
				const BestTripletPermutation& bt0 = bestTripletPermutations[toTriplets[0]][old.colsPermMask[0]];
				if (bt0.bestResult > bestTriplets0)
					continue;
				if (bt0.bestResult < bestTriplets0) {
					nNextCandidates = 0;
					bestTriplets0 = bt0.bestResult;
					bestTriplets1 = 7;
					bestTriplets2 = 7;
				}
				const BestTripletPermutation& bt1 = bestTripletPermutations[toTriplets[1]][old.colsPermMask[1]];
				if (bt1.bestResult > bestTriplets1)
					continue;
				if (bt1.bestResult < bestTriplets1) {
					nNextCandidates = 0;
					bestTriplets1 = bt1.bestResult;
					bestTriplets2 = 7;
				}
				const BestTripletPermutation& bt2 = bestTripletPermutations[toTriplets[2]][old.colsPermMask[2]];
				if (bt2.bestResult > bestTriplets2)
					continue;
				if (bt2.bestResult < bestTriplets2) {
					nNextCandidates = 0;
					bestTriplets2 = bt2.bestResult;
				}
				//tests passed, output the new candidate
				MinLexCandidate& next = nextCandidates[nNextCandidates++];
				next = old; //copy
				next.mapRowsForward[fromRow] = toRow;
				next.mapRowsBackward[toRow] = fromRow;
				next.colsPermMask[0] = bt0.resultMask;
				next.colsPermMask[1] = bt1.resultMask;
				next.colsPermMask[2] = bt2.resultMask;
			} //fromRow
		} //oldCandidateIndex
		//flip-flop the old/new
		MinLexCandidate* tmp = curCandidates;
		curCandidates = nextCandidates;
		nextCandidates = tmp;
		nCurCandidates = nNextCandidates;
		nNextCandidates = 0;
		//store the best result
		char* r = &result[9 * toRow];
		r[0] = ((bestTriplets0 >> 2) & 1);
		r[1] = ((bestTriplets0 >> 1) & 1);
		r[2] = ((bestTriplets0) & 1);
		r[3] = ((bestTriplets1 >> 2) & 1);
		r[4] = ((bestTriplets1 >> 1) & 1);
		r[5] = ((bestTriplets1) & 1);
		r[6] = ((bestTriplets2 >> 2) & 1);
		r[7] = ((bestTriplets2 >> 1) & 1);
		r[8] = ((bestTriplets2) & 1);
	} //toRow

	if (nCurCandidates == 0) {
		fprintf(stderr, "bad news: no candidates for minlex due to program errors\n");
	}

	//step 3: find the lexicographically minimal representative within the morphs,
	// this time taking into account the real values of the input givens

	Mapper map{};
	int minLex[81] = { 0 }; //the best result so far
	for (int i = 0; i < 81; i++) {
		minLex[i] = (result[i] << 5); //initially set to large values
	}
	for (int curCandidateIndex = 0; curCandidateIndex < nCurCandidates; curCandidateIndex++) {
		const MinLexCandidate& target = curCandidates[curCandidateIndex];
		int toTriplets[3] = { 0 };
		toTriplets[perm[(int)target.stacksPerm][0]] = 0;
		toTriplets[perm[(int)target.stacksPerm][1]] = 3;
		toTriplets[perm[(int)target.stacksPerm][2]] = 6;
		for (int colsPerm0 = 0; colsPerm0 < 6; colsPerm0++) {
			if (((target.colsPermMask[0] >> colsPerm0) & 1) == 0)
				continue; //forbidden permutation
			int toColsInStack[9] = { 0 };
			toColsInStack[perm[colsPerm0][0]] = toTriplets[0];
			toColsInStack[perm[colsPerm0][1]] = toTriplets[0] + 1;
			toColsInStack[perm[colsPerm0][2]] = toTriplets[0] + 2;
			for (int colsPerm1 = 0; colsPerm1 < 6; colsPerm1++) {
				if (((target.colsPermMask[1] >> colsPerm1) & 1) == 0)
					continue; //forbidden permutation
				toColsInStack[3 + perm[colsPerm1][0]] = toTriplets[1];
				toColsInStack[3 + perm[colsPerm1][1]] = toTriplets[1] + 1;
				toColsInStack[3 + perm[colsPerm1][2]] = toTriplets[1] + 2;
				for (int colsPerm2 = 0; colsPerm2 < 6; colsPerm2++) {
					if (((target.colsPermMask[2] >> colsPerm2) & 1) == 0)
						continue; //forbidden permutation
					toColsInStack[6 + perm[colsPerm2][0]] = toTriplets[2];
					toColsInStack[6 + perm[colsPerm2][1]] = toTriplets[2] + 1;
					toColsInStack[6 + perm[colsPerm2][2]] = toTriplets[2] + 2;
					int labelPerm[10] = { 0,0,0,0,0,0,0,0,0,0 }; //label mapping is unknown
					int nextFreeLabel = 1;
					int nSet = 0; //the number of givens with positions set
					for (int toRow = 0; toRow < 9; toRow++) {
						int* rowGivens = pair[(int)target.isTransposed].digits[(int)target.mapRowsBackward[toRow]];
						for (int col = 0; col < 9; col++) {
							int fromDigit = rowGivens[toColsInStack[col]];
							if (fromDigit == 0)
								continue;
							if (labelPerm[fromDigit] == 0) {
								labelPerm[fromDigit] = nextFreeLabel++;
							}
							if (labelPerm[fromDigit] > minLex[toRow * 9 + col])
								goto nextColsPerm;
							nSet++;
							if (labelPerm[fromDigit] < minLex[toRow * 9 + col]) {
								//if(minLex[toRow * 9 + col] >= (1 << 5)) { //invalidate the rest only if they are touched
								//the following puzzle demonstrates this is a bug
								//................12.....1345..6.74..1.7.8.6.348.4312.76.8..25.635..6.7.2.6.2138.57
								for (int i = toRow * 9 + col + 1; i < 81; i++) {
									minLex[i] = (result[i] << 5); //invalidate the rest
								}
								//}
								minLex[toRow * 9 + col] = labelPerm[fromDigit]; //the best result so far
								//the buffered transformations become invalid at this point
								theMaps.clear();
							}
							if (nSet == nGivens) {
								//an isomorph of the currently best ordering
								//at this point we have the necessary information for the transformation and can buffer it (if eventually it is one of the best ones)
								for (int r = 0; r < 9; r++) {
									for (int c = 0; c < 9; c++) {
										int src = target.isTransposed ? target.mapRowsBackward[r] + 9 * toColsInStack[c] : target.mapRowsBackward[r] * 9 + toColsInStack[c]; //source cell index for target rc
										map.cell[src] = minLex[r * 9 + c] ? r * 9 + c : 99; //map all non-givens to 99, this masking irrelevant permutations
									}
								}
								for (int d = 0; d < 10; d++)
									map.label[d] = 0;
								for (int d = 0; d < 10; d++)
									map.label[labelPerm[d]] = d;
								map.label[0] = 0; //don't map zero to non-zero
								theMaps.push_back(map); //this will automatically ignore duplicates
							}
						} //col
					} //toRow
				nextColsPerm:
					;
				} //colsPerm2
			} //colsPerm1
		} //colsPerm0
	} //candidate
	for (int i = 0; i < 81; i++) {
		result[i] = minLex[i] ? minLex[i] + '0' : '.'; //copy the integers to chars
	}
}

static void map(const char* src, char* results) {
	char* r = results;
	for (int i = 0; i < theMaps.size(); i++) {
		for (int j = 0; j < 81; j++) { //debug backward mapping, should display the original
			r[j] = theMaps[j].cell[j] == 99 ? 0 : theMaps[j].label[(int)src[(int)theMaps[j].cell[j]]];
		}
		r += 81;
	}
}
