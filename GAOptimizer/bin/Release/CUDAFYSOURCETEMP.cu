struct UserUpdate
{
	__device__  UserUpdate()
	{
	}
	int update_id;
	int user_id;
	int section;
	float tag;
	float timestamp;
	__device__  UserUpdate(int id, int uid, int sec, float t, float time)
	{
		update_id = id;
		user_id = uid;
		section = sec;
		tag = t;
		timestamp = time;
	}
};

struct FitnessParameter
{
	__device__  FitnessParameter()
	{
	}
	float var1;
	float var2;
	float var3;
	float var4;
	float var5;
	__device__  FitnessParameter(float variable1, float variable2, float variable3, float variable4, float variable5)
	{
		var1 = variable1;
		var2 = variable2;
		var3 = variable3;
		var4 = variable4;
		var5 = variable5;
	}
	__device__  FitnessParameter( float* variables, int variablesLen0)
	{
		var1 = variables[(0)];
		var2 = variables[(1)];
		var3 = variables[(2)];
		var4 = variables[(3)];
		var5 = variables[(4)];
	}
};

struct PredictionPerformances
{
	__device__  PredictionPerformances()
	{
	}
	float occupancyPerformance;
	float occupancyPerformanceRandom;
	float trustPerformance;
	float trustPerformanceRandom;
};

struct SimOptions
{
	__device__  SimOptions()
	{
	}
	float I;
	float lambda_promote;
	float lambda_punish;
	float certainty_coeff;
	float score_coeff;
	float decay;
	__device__  SimOptions(float val_I, float val_lambda_promote, float val_lambda_punish, float val_certainty_coeff, float val_score_coeff, float val_decay)
	{
		I = val_I;
		lambda_promote = val_lambda_promote;
		lambda_punish = val_lambda_punish;
		certainty_coeff = val_certainty_coeff;
		score_coeff = val_score_coeff;
		decay = val_decay;
	}
};


// GeneticAlgorithm.Population
extern "C" __global__  void calculateFitnessOnDevice( float* dev_fitnesses, int dev_fitnessesLen0,  float* groundTruth, int groundTruthLen0, int groundTruthLen1,  float* userTrusts, int userTrustsLen0,  UserUpdate* updates, int updatesLen0, int updatesLen1, int updatesLen2,  FitnessParameter* dev_fitnessParams, int dev_fitnessParamsLen0);
// GeneticAlgorithm.Fitness
__device__  float fitness( float* groundTruth, int groundTruthLen0, int groundTruthLen1,  float* userTrusts, int userTrustsLen0,  UserUpdate* updates, int updatesLen0, int updatesLen1, int updatesLen2, FitnessParameter fitnessParams);
// GeneticAlgorithm.Experiment
__device__  float execute( float* GroundTruth, int GroundTruthLen0, int GroundTruthLen1,  float* UserTrusts, int UserTrustsLen0,  UserUpdate* Updates, int UpdatesLen0, int UpdatesLen1, int UpdatesLen2, FitnessParameter fitnessParam);

// GeneticAlgorithm.Population
extern "C" __global__  void calculateFitnessOnDevice( float* dev_fitnesses, int dev_fitnessesLen0,  float* groundTruth, int groundTruthLen0, int groundTruthLen1,  float* userTrusts, int userTrustsLen0,  UserUpdate* updates, int updatesLen0, int updatesLen1, int updatesLen2,  FitnessParameter* dev_fitnessParams, int dev_fitnessParamsLen0)
{
	int num = blockIdx.x * blockDim.x + threadIdx.x;
	if (num < dev_fitnessesLen0)
	{
		dev_fitnesses[(num)] = fitness(groundTruth, groundTruthLen0, groundTruthLen1, userTrusts, userTrustsLen0, updates, updatesLen0, updatesLen1, updatesLen2, dev_fitnessParams[(num)]);
	}
}
// GeneticAlgorithm.Fitness
__device__  float fitness( float* groundTruth, int groundTruthLen0, int groundTruthLen1,  float* userTrusts, int userTrustsLen0,  UserUpdate* updates, int updatesLen0, int updatesLen1, int updatesLen2, FitnessParameter fitnessParams)
{
	return execute(groundTruth, groundTruthLen0, groundTruthLen1, userTrusts, userTrustsLen0, updates, updatesLen0, updatesLen1, updatesLen2, fitnessParams);
}
// GeneticAlgorithm.Experiment
__device__  float execute( float* GroundTruth, int GroundTruthLen0, int GroundTruthLen1,  float* UserTrusts, int UserTrustsLen0,  UserUpdate* Updates, int UpdatesLen0, int UpdatesLen1, int UpdatesLen2, FitnessParameter fitnessParam)
{
	SimOptions(1.0f, fitnessParam.var1, fitnessParam.var2, fitnessParam.var3, fitnessParam.var4, fitnessParam.var5);
	GroundTruthLen0;
	GroundTruthLen1;
	GroundTruthLen0;
	UpdatesLen1;
	return -1.1f;
}
