#ifndef __PARTICLE__
#define __PARTICLE__



#define DEFINE_PARTICLES_PROP(particles) StructuredBuffer<Particle> particles;



struct Particle {
    float4x4 model;
};



#endif
